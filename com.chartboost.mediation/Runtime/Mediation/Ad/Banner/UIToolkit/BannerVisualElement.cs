using System.Collections.Generic;
using System.Threading.Tasks;
using Chartboost.Logging;
using Chartboost.Mediation.Ad.Banner.Enums;
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
        
        /// <inheritdoc cref="IBannerAd.WillAppear"/>
        /// <remarks>
        /// Event type is <see cref="BannerVisualElementAdEvent"/> for UI Toolkit compatibility.
        /// </remarks>
        public event BannerVisualElementAdEvent WillAppear;

        /// <inheritdoc cref="IBannerAd.DidClick"/>
        /// <remarks>
        /// Event type is <see cref="BannerVisualElementAdEvent"/> for UI Toolkit compatibility.
        /// </remarks>
        public event BannerVisualElementAdEvent DidClick;

        /// <inheritdoc cref="IBannerAd.DidRecordImpression"/>
        /// <remarks>
        /// Event type is <see cref="BannerVisualElementAdEvent"/> for UI Toolkit compatibility.
        /// </remarks>
        public event BannerVisualElementAdEvent DidRecordImpression;

        /// <inheritdoc cref="IBannerAd.DidBeginDrag"/>
        /// <remarks>
        /// Event type is <see cref="BannerVisualElementAdDragEvent"/> for UI Toolkit compatibility.
        /// </remarks>
        public event BannerVisualElementAdDragEvent DidBeginDrag;

        /// <inheritdoc cref="IBannerAd.DidDrag"/>
        /// <remarks>
        /// Event type is <see cref="BannerVisualElementAdDragEvent"/> for UI Toolkit compatibility.
        /// </remarks>
        public event BannerVisualElementAdDragEvent DidDrag;

        /// <inheritdoc cref="IBannerAd.DidEndDrag"/>
        /// <remarks>
        /// Event type is <see cref="BannerVisualElementAdDragEvent"/> for UI Toolkit compatibility.
        /// </remarks>
        public event BannerVisualElementAdDragEvent DidEndDrag;
        
        private IBannerAd _bannerAd;

        // Native banner backing this UIToolkit variant, for automation slot hosting (HB-11391).
        internal IBannerAd NativeBannerAd => _bannerAd;

        private readonly VisualElement _ad;

        private bool _draggable;
        private bool _isDragging;
        private BannerHorizontalAlignment _horizontalAlignment = BannerHorizontalAlignment.Center;
        private BannerVerticalAlignment _verticalAlignment = BannerVerticalAlignment.Center;
        private IVisualElementScheduledItem _deferredDispose;

        /// <summary>
        /// When set, overrides the container position used for native banner sync.
        /// <see cref="SyncWithNativeContainer"/> uses this value instead of <c>worldBound</c>,
        /// providing frame-exact positioning when an external system (e.g., a draggable overlay)
        /// manages this element's screen position. Set to <c>null</c> to resume
        /// <c>worldBound</c>-based positioning.
        /// </summary>
        public Vector2? ContainerPositionOverride { get; set; }

        /// <summary>
        /// When set, overrides the container size used for native banner sync.
        /// <see cref="SyncWithNativeContainer"/> uses this value instead of <c>worldBound</c>,
        /// so a host (e.g. the automation slot) can size the native container to the slot rather
        /// than this element's creative-sized <c>worldBound</c>. Set to <c>null</c> to resume
        /// <c>worldBound</c>-based sizing.
        /// </summary>
        public Vector2? ContainerSizeOverride { get; set; }

        public BannerVisualElement()
        {
            // create ad
            _ad = new VisualElement
            {
                name = "Ad"
            };
            Add(_ad);

            // similar to update lop in MonoBehaviour
            schedule.Execute(SyncWithNativeContainer).Every((long)Time.smoothDeltaTime * 1000);

            // Dispose native ad when this is no longer part of the scene.
            // Disposal is deferred to allow reparenting (Remove + Add) without
            // destroying the native banner. See OnDetachFromPanel / OnAttachToPanel.
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
        }

        # region API
        
        /// <summary>
        /// The placement name for the ad.
        /// </summary>
        public string PlacementName { get; set; }
        
        /// <inheritdoc cref="IBannerAd.Draggable"/>
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

        /// <inheritdoc cref="IBannerAd.HorizontalAlignment"/>
        public BannerHorizontalAlignment HorizontalAlignment
        {
            get => _horizontalAlignment;
            set
            {
                if (BannerAd != null)
                    BannerAd.HorizontalAlignment = value;
                _horizontalAlignment = value;
            }
        }

        /// <inheritdoc cref="IBannerAd.VerticalAlignment"/>
        public BannerVerticalAlignment VerticalAlignment
        {
            get => _verticalAlignment;
            set
            {
                if (BannerAd != null)
                    BannerAd.VerticalAlignment = value;
                _verticalAlignment = value;
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
        
        /// <inheritdoc cref="IBannerAd.Load"/>
        public async Task<BannerAdLoadResult> Load(BannerAdLoadRequest loadRequest)
        {
            PlacementName = loadRequest.PlacementName;
            return await _bannerAd.Load(loadRequest);
        }

        /// <inheritdoc cref="IBannerAd.Reset"/>
        public void Reset() => BannerAd?.Reset();

        /// <inheritdoc cref="IBannerAd.Dispose"/>
        public void Dispose()
        {
            _deferredDispose?.Pause();
            _deferredDispose = null;
            UnsubscribeFromBannerEvents();
            // Use _bannerAd directly (not the BannerAd property) to avoid the lazy getter
            // creating a new native banner during disposal.
            _bannerAd?.Dispose();
            _bannerAd = null;
        }

        /// <summary>
        /// Positions the banner using absolute positioning based on alignment values relative to the screen.
        /// </summary>
        /// <remarks>
        /// <para><b>Why use this method:</b></para>
        /// <para>
        /// This method is designed for scenarios where you need precise, screen-relative positioning of banner ads
        /// using absolute coordinates. It's particularly useful when:
        /// <list type="bullet">
        /// <item>You want to position banners at specific screen locations (corners, edges, center)</item>
        /// <item>You're not using UIToolkit's layout system (flexbox, absolute/relative parent positioning)</item>
        /// <item>You need consistent positioning across different screen sizes and densities</item>
        /// <item>You want to dynamically reposition banners after they've loaded (e.g., for A/B testing positions)</item>
        /// </list>
        /// </para>
        ///
        /// <para><b>How it works:</b></para>
        /// <para>
        /// This method sets <c>style.position = Position.Absolute</c> and calculates pixel-perfect coordinates
        /// based on the actual banner size and screen dimensions. The alignment values (0-1 range) determine
        /// where the banner appears on screen, with fractional pixels rounded to prevent rendering issues.
        /// </para>
        ///
        /// <para><b>When NOT to use this method:</b></para>
        /// <para>
        /// If you're using UIToolkit's layout system (e.g., banner inside a flex container, using relative positioning,
        /// or styled via USS), you should control positioning through standard UIToolkit styling instead. This method
        /// overrides position-related styles with absolute positioning.
        /// </para>
        ///
        /// <para><b>Important:</b></para>
        /// <para>
        /// Must be called AFTER the banner loads (when <see cref="BannerSize"/> is available). If called before loading,
        /// a warning is logged and the method returns without effect. The container is automatically resized to match
        /// the actual banner dimensions.
        /// </para>
        /// </remarks>
        /// <param name="horizontalAlignment">Horizontal alignment (0=left, 0.5=center, 1=right)</param>
        /// <param name="verticalAlignment">Vertical alignment (0=top, 0.5=center, 1=bottom)</param>
        /// <example>
        /// </example>
        public void SetAbsolutePositionByAlignment(float horizontalAlignment, float verticalAlignment)
        {
            if (BannerSize == null)
            {
                LogController.Log("Cannot position banner - BannerSize is null. Load the banner first.", LogLevel.Warning);
                return;
            }

            // Get the actual banner size in pixels
            var nativeWidth = BannerSize.Value.Width;
            var nativeHeight = BannerSize.Value.Height;
            var width = DensityConverters.NativeToPixels(nativeWidth);
            var height = DensityConverters.NativeToPixels(nativeHeight);

            // Calculate position based on alignment
            // Use Mathf.Round() to ensure pixel-perfect positioning and avoid fractional pixel issues
            var left = Mathf.Round((Screen.width - width) * horizontalAlignment);
            var top = Mathf.Round((Screen.height - height) * verticalAlignment);

            // Apply positioning
            style.position = Position.Absolute;
            style.left = left;
            style.top = top;
            style.right = StyleKeyword.Auto;
            style.bottom = StyleKeyword.Auto;

            // Resize container to match actual banner size
            style.width = width;
            style.height = height;
        }

        #endregion
        
        /// <summary>
        /// Returns JSON representation of the current object
        /// </summary>
        public override string ToString()
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            return JsonConvert.SerializeObject(BannerAd, settings);
        }

        private IBannerAd BannerAd
        {
            get
            {
                if (_bannerAd != null)
                    return _bannerAd;

                if (Application.isPlaying)
                {
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
                    _bannerAd.HorizontalAlignment = _horizontalAlignment;
                    _bannerAd.VerticalAlignment = _verticalAlignment;
                }
                return _bannerAd;
            }
        }

        private Vector2 ContainerPosition
        {
            get
            {
                if (ContainerPositionOverride.HasValue)
                    return ContainerPositionOverride.Value;

                // worldBound gives the VisualElement's top-left position in panel coordinates
                // Convert from panel coordinates to native coordinates using UIDocToNative
                // This accounts for panel scaling via UIDocScaleFactor
                var x = DensityConverters.UIDocToNative(worldBound.x);
                var y = DensityConverters.UIDocToNative(worldBound.y);
                return new Vector2(x, y);
            }
        }

        private ContainerSize ContainerSize
        {
            get
            {
                if (ContainerSizeOverride.HasValue)
                    return ContainerSize.FixedSize((int)ContainerSizeOverride.Value.x, (int)ContainerSizeOverride.Value.y);

                // worldBound width/height are in panel coordinates
                // Convert to native dimensions
                var width = DensityConverters.UIDocToNative(worldBound.width);
                var height = DensityConverters.UIDocToNative(worldBound.height);

                return ContainerSize.FixedSize((int)width, (int)height);
            }
        }

        // True when a host (automation slot) is driving container bounds; the slot owns layout.
        private bool IsHosted => ContainerPositionOverride.HasValue || ContainerSizeOverride.HasValue;

        private Vector2 AdRelativePosition
        {
            get
            {
                // _ad.layout gives position relative to parent (the BannerVisualElement) in panel coordinates
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
            // Use _bannerAd directly (not the BannerAd property) to avoid the lazy getter
            // creating a new native banner after Dispose() has nulled the field.
            if (_bannerAd == null) 
                return;
            
            if (!ContainerSize.Equals(_bannerAd.ContainerSize))
                _bannerAd.ContainerSize = ContainerSize;

            if (ContainerPosition != _bannerAd.Position && !_isDragging)
                _bannerAd.Position = ContainerPosition;

            if (ContainerVisible != _bannerAd.Visible)
                _bannerAd.Visible = ContainerVisible;

            // AdRelativePosition pins the creative absolutely within the container. When hosted in
            // the native slot, let the native Center alignment position it (mirrors med, which never
            // sets this); pushing a value here would override centering and pin it top-left.
            if (!IsHosted && AdRelativePosition != ((BannerAdBase)_bannerAd).AdRelativePosition)
                ((BannerAdBase)_bannerAd).AdRelativePosition = AdRelativePosition;
        }

        private void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            // Defer disposal to allow reparenting. When an element is moved between parents
            // (e.g., during drag detach/reattach), DetachFromPanelEvent fires during the Remove
            // step. If the element is re-attached before the deferred callback executes,
            // OnAttachToPanel cancels the disposal.
            // Schedule on the origin panel's visual tree since this element is no longer in a panel.
            _deferredDispose = evt.originPanel?.visualTree?.schedule?.Execute(() =>
            {
                _deferredDispose = null;
                if (panel == null)
                    Dispose();
            });

            // If the origin panel's scheduler is unavailable (panel was destroyed), dispose immediately.
            if (_deferredDispose == null && panel == null)
                Dispose();
        }

        private void OnAttachToPanel(AttachToPanelEvent evt)
        {
            // Element was re-attached (reparented, not removed) — cancel pending disposal.
            _deferredDispose?.Pause();
            _deferredDispose = null;
        }

        /// <summary>
        /// Unsubscribes from all banner ad events to prevent memory leaks.
        /// </summary>
        private void UnsubscribeFromBannerEvents()
        {
            if (_bannerAd == null)
                return;

            _bannerAd.WillAppear -= OnWillAppear;
            _bannerAd.DidClick -= OnClick;
            _bannerAd.DidRecordImpression -= OnRecordImpression;
            _bannerAd.DidBeginDrag -= OnDragBegin;
            _bannerAd.DidDrag -= OnDrag;
            _bannerAd.DidEndDrag -= OnDragEnd;
        }
    }
}
