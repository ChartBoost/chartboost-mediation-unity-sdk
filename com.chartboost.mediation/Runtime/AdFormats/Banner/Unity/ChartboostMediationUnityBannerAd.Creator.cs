using Chartboost.Banner;
using Chartboost.Utilities;
using UnityEditor;
using UnityEngine;

namespace Chartboost.AdFormats.Banner.Unity
{
    public partial class ChartboostMediationUnityBannerAd : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("GameObject/Chartboost Mediation/UnityBannerAd")]
        public static void CreateAd()
        {
            Instantiate();
        }
#endif

        internal static ChartboostMediationUnityBannerAd Instantiate(
            Canvas canvas = null,
            ChartboostMediationBannerSize? size = null,
            ChartboostMediationBannerAdScreenLocation screenLocation = ChartboostMediationBannerAdScreenLocation.Center)
        {
            canvas ??= ChartboostMediationUtils.GetCanvas();
            
            // Instantiate inside this canvas
            var unityBannerAd = new GameObject("ChartboostMediationUnityBannerAd")
                .AddComponent<ChartboostMediationUnityBannerAd>();
            
            var bannerTransform = unityBannerAd.transform;
            bannerTransform.parent = canvas.transform;
            bannerTransform.localScale = Vector3.one;

            // If no size is provided use Standard size as default
            var containerSize = size ?? ChartboostMediationBannerSize.Standard;
            unityBannerAd.SetSizeType(containerSize.SizeType);
            
            var canvasScale = canvas.transform.localScale.x;
            var width = ChartboostMediationConverters.NativeToPixels(containerSize.Width)/canvasScale;
            var height = ChartboostMediationConverters.NativeToPixels(containerSize.Height)/canvasScale;
            
            var rectTransform = unityBannerAd.gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(width, height);

            PlaceUnityBannerAd(unityBannerAd, screenLocation);
            
            return unityBannerAd;
        }
        
        private static void PlaceUnityBannerAd(ChartboostMediationUnityBannerAd unityBannerAd,
            ChartboostMediationBannerAdScreenLocation screenLocation)
        {
            var left = Screen.safeArea.xMin / Screen.width;
            var right = Screen.safeArea.xMax / Screen.width;
            var top = Screen.safeArea.yMax / Screen.height;
            var bottom = Screen.safeArea.yMin / Screen.height;
            var center = 0.5f;
            
            var pivot = Vector2.zero;
            var anchor = Vector2.zero;
            switch (screenLocation)
            {
                case ChartboostMediationBannerAdScreenLocation.TopLeft:
                    anchor = new Vector2(left, top);
                    pivot = new Vector2(0, 1);
                    break;
                case ChartboostMediationBannerAdScreenLocation.TopCenter:
                    anchor = new Vector2(center, top);
                    pivot = new Vector2(0.5f, 1);
                    break;
                case ChartboostMediationBannerAdScreenLocation.TopRight:
                    anchor = new Vector2(right, top);
                    pivot = new Vector2(1, 1);
                    break;
                case ChartboostMediationBannerAdScreenLocation.Center:
                    anchor = new Vector2(center, center);
                    pivot = new Vector2(0.5f, 0.5f);
                    break;
                case ChartboostMediationBannerAdScreenLocation.BottomLeft:
                    anchor = new Vector2(left, bottom);
                    pivot = new Vector2(0, 0);
                    break;
                case ChartboostMediationBannerAdScreenLocation.BottomCenter:
                    anchor = new Vector2(center, bottom);
                    pivot = new Vector2(0.5f, 0);
                    break;
                case ChartboostMediationBannerAdScreenLocation.BottomRight:
                    anchor = new Vector2(right, bottom);
                    pivot = new Vector2(1, 0);
                    break;
            }
            
            var rect = unityBannerAd.GetComponent<RectTransform>();

            rect.anchorMin = rect.anchorMax = anchor;
            rect.pivot = pivot;
            rect.anchoredPosition = Vector2.zero;
        }
    }
}
