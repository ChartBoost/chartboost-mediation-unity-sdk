#if UNITY_IOS
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace Chartboost.Banner
{
    /// <summary>
    /// Chartboost Mediation banner object for iOS.
    /// </summary>
    public class ChartboostMediationBannerIOS : ChartboostMediationBannerBase
    {
        public Action<float, float> dragListener;

        /// <summary>
        /// uniqueId obtained from native that can be associated with this object 
        /// </summary>
        private readonly IntPtr _uniqueId;
        
        /// <summary>
        /// Maps uniqueIds from native with objects of this class
        /// </summary>
        private static Dictionary<IntPtr, ChartboostMediationBannerIOS> _adObjects;
        
        /// <summary>
        /// callback definition for drag event on objective-c layer
        /// </summary>
        private delegate void ExternChartboostMediationBannerDragEvent(IntPtr uniqueId, float x, float y);

        public ChartboostMediationBannerIOS(string placement, ChartboostMediationBannerAdSize size) : base(placement, size)
        {
            LogTag = "ChartboostMediation Banner (iOS)";
            _uniqueId = _chartboostMediationGetBannerAd(placement, (int)size);
            
            _adObjects ??= new Dictionary<IntPtr, ChartboostMediationBannerIOS>();
            if (!_adObjects.ContainsKey(_uniqueId))
            {
                _adObjects.Add(_uniqueId, null);
            }
            _adObjects[_uniqueId] = this;
        }

        /// <inheritdoc cref="ChartboostMediationBannerBase.SetKeyword"/>>
        public override bool SetKeyword(string keyword, string value)
        {
            base.SetKeyword(keyword, value);
            return _chartboostMediationBannerSetKeyword(_uniqueId, keyword, value);
        }

        /// <inheritdoc cref="ChartboostMediationBannerBase.RemoveKeyword"/>>
        public override string RemoveKeyword(string keyword)
        {
            base.RemoveKeyword(keyword);
            return _chartboostMediationBannerRemoveKeyword(_uniqueId, keyword);
        }

        /// <inheritdoc cref="ChartboostMediationBannerBase.Load"/>>
        public override void Load(ChartboostMediationBannerAdScreenLocation location)
        {
            base.Load(location);
            _chartboostMediationBannerAdLoad(_uniqueId, (int)location);
        }

        /// <inheritdoc cref="ChartboostMediationBannerBase.Load(float, float, int, int)"/>>
        public override void Load(float x, float y, int width, int height)
        {
            base.Load(x, y, width, height);
            _chartboostMediationBannerAdLoadWithParams(_uniqueId, x, Screen.height - y, width, height);
        }

        /// <inheritdoc cref="ChartboostMediationBannerBase.SetVisibility"/>>
        public override void SetVisibility(bool isVisible)
        {
            base.SetVisibility(isVisible);
            _chartboostMediationBannerSetVisibility(_uniqueId, isVisible);
        }

        /// <inheritdoc cref="ChartboostMediationBannerBase.ClearLoaded"/>>
        public override void ClearLoaded()
        {
            base.ClearLoaded();
            _chartboostMediationBannerClearLoaded(_uniqueId);
        }

        /// <inheritdoc cref="ChartboostMediationBannerBase.Remove"/>>
        public override void Remove()
        {
            base.Remove();
            
            if (_adObjects.ContainsKey(_uniqueId))
            {
                _adObjects.Remove(_uniqueId);
            }
            _chartboostMediationBannerRemove(_uniqueId);
        }

        public override void EnableDrag(Action<float, float> onDrag = null)
        {
            base.EnableDrag(onDrag);
            dragListener = onDrag;

            _chartboostMediationBannerEnableDrag(_uniqueId, ExternBannerDragListener);
        }

        public override void DisableDrag()
        {
            base.DisableDrag();
            _chartboostMediationBannerDisableDrag(_uniqueId);
        }


        ~ChartboostMediationBannerIOS()
            => _chartboostMediationFreeAdObject(_uniqueId, placementName, true);

        #region External Methods
        [DllImport("__Internal")]
        private static extern IntPtr _chartboostMediationGetBannerAd(string placementName, int size);
        [DllImport("__Internal")]
        private static extern bool _chartboostMediationBannerSetKeyword(IntPtr uniqueId, string keyword, string value);
        [DllImport("__Internal")]
        private static extern string _chartboostMediationBannerRemoveKeyword(IntPtr uniqueID, string keyword);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerAdLoad(IntPtr uniqueID, int screenLocation);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerAdLoadWithParams(IntPtr uniqueID, float x, float y, int width, int height);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerClearLoaded(IntPtr uniqueID);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerRemove(IntPtr uniqueID);
        [DllImport("__Internal")]
        private static extern bool _chartboostMediationBannerSetVisibility(IntPtr uniqueID, bool isVisible);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationFreeAdObject(IntPtr uniqueID, string placementName, bool multiPlacementSupport);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerEnableDrag(IntPtr uniqueID, ExternChartboostMediationBannerDragEvent draglistener);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerDisableDrag(IntPtr uniqueID);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationBannerDragEvent))]
        private static void ExternBannerDragListener(IntPtr uniqueId, float x, float y)
        {
            // Get associated adObject to this uniqueId and invoke it's dragListener
            if (_adObjects.TryGetValue(uniqueId, out var adObject))
            {
                adObject.dragListener?.Invoke(x, Screen.height - y);
            }
            else
            {
                throw new Exception("Banner object not found");
            }
        }
        #endregion
    }
}
#endif
