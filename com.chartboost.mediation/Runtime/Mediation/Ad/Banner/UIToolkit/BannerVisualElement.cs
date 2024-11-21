using System.Collections.Generic;
using System.Threading.Tasks;
using Chartboost.Logging;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chartboost.Mediation.Ad.Banner.UIToolkit
{
    /// <summary>
    /// Unity UIToolkit compatible <see cref="IBannerAd"/>.
    /// </summary>
    public class BannerVisualElement : VisualElement, IAd
    {
        public new class UxmlFactory : UxmlFactory<BannerVisualElement, UxmlTraits> { }
        
        /// <summary>
        /// Called when ad is loaded within this VisualElement. This will be called for each refresh when auto-refresh is enabled.
        /// </summary>
        public event BannerVisualElementAdEvent WillAppear;
        
        /// <summary>
        /// Called when the ad executes its click-through. This may happen multiple times for the same ad.
        /// </summary>
        public event BannerVisualElementAdEvent DidClick;
        
        /// <summary>
        /// Called when the ad impression occurs.
        /// </summary>
        public event BannerVisualElementAdEvent DidRecordImpression;
        
        /// <summary>
        ///  Called when this VisualElement has begun dragging on screen.
        /// </summary>
        public event BannerVisualElementAdDragEvent DidBeginDrag;
        
        /// <summary>
        ///  Called when this VisualElement is dragged on screen.
        /// </summary>
        public event BannerVisualElementAdDragEvent DidDrag;
        
        /// <summary>
        ///  Called when this VisualElement has finished dragging on screen.
        /// </summary>
        public event BannerVisualElementAdDragEvent DidEndDrag;
        
        private IBannerAd _bannerAd;
        private readonly VisualElement _ad;

        private bool _draggable;
        private bool _isDragging;

        public BannerVisualElement()
        {
            // create ad
            _ad = new VisualElement
            {
                name = "Ad"
            };
            Add(_ad);

            // similar to update lop in Monobehaviour
            schedule.Execute(SyncWithNativeContainer).Every((long)Time.smoothDeltaTime * 1000);

            // Dispose native ad when this is no longer part of the scene
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        # region API
        
        /// <summary>
        /// The placement name for the ad.
        /// </summary>
        public string PlacementName { get; set; }
        
        /// <summary>
        /// The ability of this VisualElement to drag
        /// </summary>
        public bool Draggable
        {
            get => _draggable;
            set
            {
                if (BannerAd != null)
                    BannerAd.Draggable = value;
                _draggable = value;
            }
        }

        /// <inheritdoc cref="IBannerAd.Keywords"/>
        public IReadOnlyDictionary<string, string> Keywords
        {
            get => BannerAd?.Keywords;
            set => BannerAd.Keywords = value;
        }
        
        /// <inheritdoc cref="IBannerAd.PartnerSettings"/>
        public IReadOnlyDictionary<string, string> PartnerSettings
        {
            get => BannerAd?.PartnerSettings;
            set => BannerAd.PartnerSettings = value;
        }
        
        /// <inheritdoc cref="IBannerAd.Request"/>
        public BannerAdLoadRequest Request => BannerAd?.Request;
        
        /// <inheritdoc cref="IBannerAd.WinningBidInfo"/>
        public BidInfo? WinningBidInfo => BannerAd?.WinningBidInfo;
        
        /// <inheritdoc cref="IBannerAd.LoadMetrics"/>
        public Metrics? LoadMetrics => BannerAd?.LoadMetrics;
        
        /// <inheritdoc cref="IBannerAd.LoadId"/>
        public string LoadId => BannerAd?.LoadId;
        
        /// <inheritdoc cref="IBannerAd.BannerSize"/>
        public BannerSize? BannerSize => BannerAd?.BannerSize;

        /// <summary>
        /// Loads an ad inside this VisualElement.
        /// Uses the size of this VisualElement (width and height in pixels) to construct the
        /// <see cref="Banner.BannerSize"/> in load request 
        /// </summary>
        /// <returns></returns>
        public async Task<BannerAdLoadResult> Load()
        {
            var size = Banner.BannerSize.Adaptive(DensityConverters.UIDocToNative(worldBound.width), DensityConverters.UIDocToNative(worldBound.height));
            var loadRequest = new BannerAdLoadRequest(PlacementName, size);
            return await _bannerAd.Load(loadRequest);
        }
        
        /// <summary>
        /// Loads an ad inside this VisualElement.
        /// </summary>
        /// <param name="loadRequest"></param>
        /// <returns></returns>
        public async Task<BannerAdLoadResult> Load(BannerAdLoadRequest loadRequest)
        {
            PlacementName = loadRequest.PlacementName;
            return await _bannerAd.Load(loadRequest);
        }

        /// <summary>
        /// Clears the loaded ad
        /// </summary>
        public void Reset() => BannerAd?.Reset();

        /// <inheritdoc cref="IBannerAd.Dispose"/>
        public void Dispose() => BannerAd?.Dispose();

        #endregion
        
        /// <summary>
        /// Returns json representation of current state of the object
        /// </summary>
        public override string ToString() => JsonConvert.SerializeObject(BannerAd);

        private IBannerAd BannerAd
        {
            get
            {
                if (_bannerAd != null)
                    return _bannerAd;

                if (!Application.isPlaying)
                    return null;

                _bannerAd = ChartboostMediation.GetBannerAd();
                _bannerAd.WillAppear += OnWillAppear;
                _bannerAd.DidClick += OnClick;
                _bannerAd.DidRecordImpression += OnRecordImpression;
                _bannerAd.DidBeginDrag += OnDragBegin;
                _bannerAd.DidDrag += OnDrag;
                _bannerAd.DidEndDrag += OnDragEnd;

                // Initialize
                _bannerAd.Visible = style.display.value == DisplayStyle.Flex;
                _bannerAd.Position = ContainerPosition;
                _bannerAd.ContainerSize = ContainerSize;
                _bannerAd.Draggable = Draggable;

                return _bannerAd;
            }
        }

        private Vector2 ContainerPosition
        {
            get
            {
                var x = DensityConverters.UIDocToNative(worldBound.x);
                var y = DensityConverters.UIDocToNative(worldBound.y);
                return new Vector2(x, y);
            }
        }

        private ContainerSize ContainerSize
        {
            get
            {
                var x = DensityConverters.UIDocToNative(worldBound.width);
                var y = DensityConverters.UIDocToNative(worldBound.height);

                return ContainerSize.FixedSize((int)x, (int)y);
            }
        }

        private Vector2 AdRelativePosition
        {
            get
            {
                var x = DensityConverters.UIDocToNative(_ad.layout.x);
                var y = DensityConverters.UIDocToNative(_ad.layout.y);
                return new Vector2(x, y);
            }
        }

        private bool ContainerVisible => style.display.value == DisplayStyle.Flex;

        private void OnWillAppear(IBannerAd bannerAd)
        {
            var width = bannerAd.BannerSize?.Width ?? 0;
            var height = bannerAd.BannerSize?.Height ?? 0;
            _ad.style.width = width;
            _ad.style.height = height;

            WillAppear?.Invoke(this);
        }

        private void OnRecordImpression(IBannerAd bannerView) => DidRecordImpression?.Invoke(this);

        private void OnClick(IBannerAd bannerView) => DidClick?.Invoke(this);

        private void OnDragBegin(IBannerAd bannerAdd, float x, float y)
        {
            _isDragging = true;
            DidBeginDrag?.Invoke(this, x, y);
        }

        private void OnDrag(IBannerAd bannerView, float x, float y)
        {
            if (!_isDragging)
            {
                LogController.Log("The DidDrag event was triggered, but no preceding DidDragBegin event was detected.", LogLevel.Debug);
                return;
            }

            style.position = Position.Absolute;
            var localPosition = parent.WorldToLocal(new Vector2(DensityConverters.PixelsToUIDoc(x), DensityConverters.PixelsToUIDoc(y)));

            style.left = localPosition.x;
            style.top = localPosition.y;

            DidDrag?.Invoke(this, x, y);
        }

        private void OnDragEnd(IBannerAd bannerAd, float x, float y)
        {
            _isDragging = false;
            DidEndDrag?.Invoke(this, x, y);
        }

        private void SyncWithNativeContainer()
        {
            if (BannerAd == null)
                return;

            if (!ContainerSize.Equals(BannerAd.ContainerSize))
                BannerAd.ContainerSize = ContainerSize;

            if (ContainerPosition != BannerAd.Position && !_isDragging)
                BannerAd.Position = ContainerPosition;

            if (ContainerVisible != BannerAd.Visible)
                BannerAd.Visible = ContainerVisible;

            // AdRelativePosition is an internal function
            if (AdRelativePosition != ((BannerAdBase)BannerAd).AdRelativePosition)
                ((BannerAdBase)BannerAd).AdRelativePosition = AdRelativePosition;
        }

        private void OnDetachFromPanel(DetachFromPanelEvent evt) => BannerAd?.Dispose();

    }
}
