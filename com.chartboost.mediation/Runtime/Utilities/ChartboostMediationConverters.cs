#if UNITY_ANDROID && !UNITY_EDITOR
using Chartboost.Platforms.Android;
#elif UNITY_IPHONE && !UNITY_EDITOR
using Chartboost.Platforms.IOS;
#endif
using UnityEngine;

namespace Chartboost.Utilities
{
    public static class ChartboostMediationConverters
    {
        /// <summary>
        /// Multiplication factor that can be applied to native unit (dips in Android and points in iOS) to get the value in pixels.
        /// </summary>
        public static readonly float EditorUIScaleFactor = 2.5f;
        
        private static float? _scaleFactor = null;
        
        public static float NativeToPixels(float native)
        {
            return native * ScaleFactor;
        }
        
        public static float PixelsToNative(float pixels)
        {
            return pixels / ScaleFactor;
        }

        public static Vector2 NativeToPixels(Vector2 native)
        {
            return new Vector2(NativeToPixels(native.x), NativeToPixels(native.y));
        }
        
        public static Vector2 PixelsToNative(Vector2 pixels)
        {
            return new Vector2(PixelsToNative(pixels.x), NativeToPixels(pixels.y));
        }

        private static float ScaleFactor
        {
            get
            {
                #if UNITY_EDITOR
                return _scaleFactor ??= EditorUIScaleFactor;
                #elif UNITY_ANDROID
                return _scaleFactor ??= ChartboostMediationAndroid.GetUIScaleFactor();
                #elif UNITY_IPHONE
                return _scaleFactor ??= ChartboostMediationIOS.GetUIScaleFactor();
                #else
                return _scaleFactor ??= Constants.EditorUIScaleFactor;
                #endif
            }
        }
        
    }
}
