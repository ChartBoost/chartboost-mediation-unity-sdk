#if UNITY_ANDROID
using Chartboost.Platforms.Android;
#elif UNITY_IPHONE
using Chartboost.Platforms.IOS;
#endif
using UnityEngine;

namespace Chartboost.Utilities
{
    public static class ChartboostMediationConverters
    {
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
                return _scaleFactor ??= Constants.EditorUIScaleFactor;
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
