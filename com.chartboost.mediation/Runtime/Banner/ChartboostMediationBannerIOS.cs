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

        private readonly IntPtr _uniqueId;

        private static Dictionary<IntPtr, ChartboostMediationBannerIOS> listeners;
        

        private delegate void ExternChartboostMediationBannerDragEvent(IntPtr uniqueId, float x, float y);

        public ChartboostMediationBannerIOS(string placement, ChartboostMediationBannerAdSize size) : base(placement, size)
        {
            LogTag = "ChartboostMediation Banner (iOS)";
            _uniqueId = _chartboostMediationGetBannerAd(placement, (int)size);
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
            _chartboostMediationBannerRemove(_uniqueId);
        }

        /// <inheritdoc cref="IChartboostMediationBannerAd.SetParams(float, float, int, int)"/>>
        public override void SetParams(float x, float y, int width, int height)
        {
            base.SetParams(x, y, width, height);
            _chartboostMediationBannerAdSetlayoutParams(_uniqueId, x, Screen.height - y, width, height);  // Android measures pixels from top whereas Unity provides measurement from bottom of screen
        }

        public override void EnableDrag(Action<float, float> didDrag = null)
        {
            base.EnableDrag(didDrag);

            dragListener = didDrag;

            if(listeners == null)
            {
                listeners = new Dictionary<IntPtr, ChartboostMediationBannerIOS>();
            }

            if (!listeners.ContainsKey(_uniqueId))
            {
                listeners.Add(_uniqueId, null);
            }

            listeners[_uniqueId] = this;

            _chartboostMediationBannerEnableDrag(_uniqueId, ExternBannerDragListener);


        }

        public override void DisableDrag()
        {
            base.DisableDrag();
            
            if (listeners.ContainsKey(_uniqueId))
            {
                listeners.Remove(_uniqueId);
            }

            _chartboostMediationBannerDisableDrag(_uniqueId);

        }


        ~ChartboostMediationBannerIOS()
            => _chartboostMediationFreeBannerAdObject(_uniqueId);

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
        private static extern void _chartboostMediationFreeBannerAdObject(IntPtr uniqueID);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerAdSetlayoutParams(IntPtr uniqueID, float x, float y, int width, int height);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerEnableDrag(IntPtr uniqueID, ExternChartboostMediationBannerDragEvent draglistener);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerDisableDrag(IntPtr uniqueID);

        #endregion

        [MonoPInvokeCallback(typeof(ExternChartboostMediationBannerDragEvent))]
        private static void ExternBannerDragListener(IntPtr uniqueId, float x, float y)
        {
            Debug.Log($"Dragger Callback in unity {x} , {y}");

            //GCHandle handle = (GCHandle)uniqueId;
            //ChartboostMediationBannerIOS banner = handle.Target as ChartboostMediationBannerIOS;

            //Debug.Log($"retrieved banner object for placement {banner._uniqueId}");
            //banner.dragListener?.Invoke(x, y);

            if (listeners.ContainsKey(uniqueId))
            {
                Debug.Log("Banner object found");
                listeners[uniqueId].dragListener?.Invoke(x, Screen.height - y);
            }
            else
            {
                Debug.Log("Banner object not found");
            }
        }
    }
}
#endif
