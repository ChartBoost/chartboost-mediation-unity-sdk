using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Chartboost.Banner;
using Chartboost.Requests;
using Chartboost.Results;
using Chartboost.Utilities;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Logger = Chartboost.Utilities.Logger;

namespace Chartboost.AdFormats.Banner.Unity
{
    public delegate void ChartboostMediationUnityBannerAdEvent();
    public delegate void ChartboostMediationUnityBannerAdDragEvent(float x, float y);
    
    public partial class ChartboostMediationUnityBannerAd : MonoBehaviour
    {
        public ChartboostMediationUnityBannerAdEvent WillAppear;
        public ChartboostMediationUnityBannerAdEvent DidClick;
        public ChartboostMediationUnityBannerAdEvent DidRecordImpression;
        public ChartboostMediationUnityBannerAdDragEvent DidDrag;
        
        [SerializeField] 
        private bool autoLoadOnInit = true;

        [SerializeField] 
        private string placementName;
        [SerializeField] 
        private bool draggable = true;
        
        [SerializeField][HideInInspector] 
        private UnityBannerAdSize size;
        [SerializeField][HideInInspector] 
        private bool resizeToFit;
        [SerializeField][HideInInspector] 
        private ChartboostMediationBannerHorizontalAlignment horizontalAlignment = ChartboostMediationBannerHorizontalAlignment.Center;
        [SerializeField][HideInInspector] 
        private ChartboostMediationBannerVerticalAlignment verticalAlignment = ChartboostMediationBannerVerticalAlignment.Center;
        
        private IChartboostMediationBannerView _bannerView;

        # region Unity Lifecycle
        private void Start()
        {
            ChartboostMediation.DidStart += ChartboostMediationOnDidStart;
        }

        private void OnEnable()
        {
            BannerView?.SetVisibility(true);
        }

        private void OnDisable()
        {
            BannerView?.SetVisibility(false);
        }

        public void OnDestroy() => BannerView?.Destroy();
        
        #endregion

        public override string ToString()
        {
            base.ToString();
            return JsonConvert.SerializeObject(BannerView);
        }

        public string PlacementName
        {
            get => placementName;
            internal set => placementName = value;
        }
        public bool Draggable
        {
            get => draggable;
            set
            {
                BannerView.SetDraggability(draggable);
                draggable = value;
            }
        }
        public bool ResizeToFit { get => resizeToFit; set => resizeToFit = value; }
        public async Task<ChartboostMediationBannerAdLoadResult> Load()
        {
            if (string.IsNullOrEmpty(placementName))
            {
                const string error = "Placement Name is empty or not set in inspector";
                Logger.LogError("ChartboostMediationUnityBannerAd",  error);
                return new ChartboostMediationBannerAdLoadResult(new ChartboostMediationError(error));
            }
            
            var recTransform = GetComponent<RectTransform>();
            var layoutParams = recTransform.LayoutParams();
            if (recTransform.GetComponentInParent<LayoutGroup>())
            {
                // Note : if rectTransform is part of a layoutgroup then we need wait until the layout is created
                // https://forum.unity.com/threads/solved-cant-get-the-rect-width-rect-height-of-an-element-when-using-layouts.377953/
                while (layoutParams.width == 0 && layoutParams.height == 0) // TODO: Find a better approach => use minWidth and minHeight instead of 0s ?
                {
                    await Task.Yield();
                    layoutParams = recTransform.LayoutParams();
                }
            }
            
            var width = ChartboostMediationConverters.PixelsToNative(layoutParams.width);
            var height = ChartboostMediationConverters.PixelsToNative(layoutParams.height);

            var bannerViewSize = this.size switch
            {
                UnityBannerAdSize.Standard => ChartboostMediationBannerAdSize.Standard,
                UnityBannerAdSize.Medium => ChartboostMediationBannerAdSize.MediumRect,
                UnityBannerAdSize.Leaderboard => ChartboostMediationBannerAdSize.Leaderboard,
                _ => ChartboostMediationBannerAdSize.Adaptive(width, height)
            };

            var loadRequest = new ChartboostMediationBannerAdLoadRequest(placementName, bannerViewSize);
            
            var x = ChartboostMediationConverters.PixelsToNative(layoutParams.x);
            var y = ChartboostMediationConverters.PixelsToNative(layoutParams.y);
            
            return await BannerView.Load(loadRequest, x, y);
        }
        
        #region BannerView Wrap
        public Dictionary<string, string> Keywords
        {
            get => BannerView?.Keywords;
            set => BannerView.Keywords = value;
        }

        public ChartboostMediationBannerAdLoadRequest Request => BannerView?.Request;
        public BidInfo? WinningBidInfo => BannerView?.WinningBidInfo;
        public string LoadId => BannerView?.LoadId;
        public ChartboostMediationBannerAdSize AdSize => BannerView?.AdSize;
        public ChartboostMediationBannerHorizontalAlignment HorizontalAlignment
        {
            get => horizontalAlignment;
            set
            {
                BannerView.HorizontalAlignment = value;
                horizontalAlignment = value;
            }
        }
        public ChartboostMediationBannerVerticalAlignment VerticalAlignment
        {
            get => verticalAlignment;
            set
            {
                BannerView.VerticalAlignment = value;
                verticalAlignment = value;
            }
        }
        public void ResetAd() => BannerView.Reset();
        #endregion
        
        #region Events

        private void OnWillAppear(IChartboostMediationBannerView bannerView)
        {
            WillAppear?.Invoke();
            if (ResizeToFit)
            {
                var canvas = GetComponentInParent<Canvas>();
                var canvasScale = canvas.transform.localScale.x;
                var width = ChartboostMediationConverters.NativeToPixels(AdSize.Width)/canvasScale;
                var height = ChartboostMediationConverters.NativeToPixels(AdSize.Height)/canvasScale;
                var rect = GetComponent<RectTransform>();
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
        }
        private void OnRecordImpression(IChartboostMediationBannerView bannerView) => DidRecordImpression?.Invoke();
        private void OnClick(IChartboostMediationBannerView bannerView) => DidClick?.Invoke();
        private async void ChartboostMediationOnDidStart(string error)
        {
            if (string.IsNullOrEmpty(error))
            {
                if (autoLoadOnInit)
                {
                    await Load();
                }
            }
        }
        private void OnDrag(IChartboostMediationBannerView bannerView, float x, float y)
        {
            transform.position = new Vector3(x, y, 0);
            DidDrag?.Invoke(x, y);
        }

        #endregion

        internal void SetUnityBannerAdSize(UnityBannerAdSize size)
        {
            this.size = size;
        }
        private IChartboostMediationBannerView BannerView
        {
            get
            {
                if (_bannerView == null)
                {
                    _bannerView = ChartboostMediation.GetBannerView();
                    _bannerView.WillAppear +=OnWillAppear;
                    _bannerView.DidClick += OnClick;
                    _bannerView.DidRecordImpression += OnRecordImpression;
                    _bannerView.DidDrag += OnDrag;
                }
        
                return _bannerView;
            }
        }
        
        private ChartboostMediationBannerAdSize GetSize(float width, float height)
        {
            var size = ChartboostMediationBannerAdSize.Adaptive(width, height);

            // TODO: Remove?
            if (Math.Abs(width - 320) < .5f && Math.Abs(height - 50) < .5f)
            {
                size = ChartboostMediationBannerAdSize.Standard;
            }
            else if (Math.Abs(width - 300) < .5f && Math.Abs(height - 250) < .5f)
            {
                size = ChartboostMediationBannerAdSize.MediumRect;
            }
            else if (Math.Abs(width - 728) < .5f && Math.Abs(height - 90) < .5f)
            {
                size = ChartboostMediationBannerAdSize.Leaderboard;
            }

            return size;
        }
        
    }
    
    #if UNITY_EDITOR

    public enum UnityBannerAdSize
    {
        Adaptive,
        Standard,
        Medium,
        Leaderboard
    }
    
    [CustomEditor(typeof(ChartboostMediationUnityBannerAd))]
    internal class ChartboostMediationUnityBannerAdEditor : Editor
    {
        private SerializedProperty _sizeSP;
        private SerializedProperty _horizontalAlignmentSP;
        private SerializedProperty _verticalAlignmentSP;
        private SerializedProperty _resizeToFitSP;
        
        // Fixed
        private UnityBannerAdSize _size = UnityBannerAdSize.Standard;
        
        // Adaptive
        private bool _resizeToFit;
        private ChartboostMediationBannerHorizontalAlignment _horizontalAlignment = ChartboostMediationBannerHorizontalAlignment.Center;
        private ChartboostMediationBannerVerticalAlignment _verticalAlignment = ChartboostMediationBannerVerticalAlignment.Center;

        private void OnEnable()
        {
            _sizeSP = serializedObject.FindProperty("size");
            _horizontalAlignmentSP = serializedObject.FindProperty("horizontalAlignment");
            _verticalAlignmentSP = serializedObject.FindProperty("verticalAlignment");
            _resizeToFitSP = serializedObject.FindProperty("resizeToFit");

            _size = (UnityBannerAdSize)_sizeSP.intValue;
            _resizeToFit = _resizeToFitSP.boolValue;
            _horizontalAlignment = (ChartboostMediationBannerHorizontalAlignment)_horizontalAlignmentSP.intValue;
            _verticalAlignment = (ChartboostMediationBannerVerticalAlignment)_verticalAlignmentSP.intValue;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            _size = (UnityBannerAdSize)EditorGUILayout.EnumPopup("Size", _size);
            
            if (_size == (int)UnityBannerAdSize.Adaptive)
            {
                _resizeToFit = EditorGUILayout.Toggle("Resize To Fit", _resizeToFit);
                
                if (!_resizeToFitSP.boolValue)
                {
                    _horizontalAlignment = (ChartboostMediationBannerHorizontalAlignment)EditorGUILayout.EnumPopup("Horizontal Alignment", _horizontalAlignment);
                    _verticalAlignment = (ChartboostMediationBannerVerticalAlignment)EditorGUILayout.EnumPopup("Vertical Alignment", _verticalAlignment);
                }
            }
            
            _sizeSP.intValue = (int)_size;
            _resizeToFitSP.boolValue = _resizeToFit;
            _horizontalAlignmentSP.intValue = (int)_horizontalAlignment;
            _verticalAlignmentSP.intValue = (int)_verticalAlignment;

            serializedObject.ApplyModifiedProperties();
        }
    }
    
    #endif
    
}