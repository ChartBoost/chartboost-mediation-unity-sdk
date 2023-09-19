using Chartboost.Banner;
using Chartboost.Utilities;
using UnityEditor;
using UnityEngine;
using static Chartboost.Utilities.Constants;

namespace Chartboost.AdFormats.Banner.Unity
{
    public partial class ChartboostMediationUnityBannerAd : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("Chartboost Mediation/UnityBannerAd/Create New")]
        [MenuItem("GameObject/Chartboost Mediation/UnityBannerAd")]
        public static void CreateAd()
        {
            Instantiate();
        }
#endif

        internal static ChartboostMediationUnityBannerAd Instantiate(
            ChartboostMediationBannerAdSize size = null,
            ChartboostMediationBannerAdScreenLocation screenLocation = ChartboostMediationBannerAdScreenLocation.Center)
        {
            // Find canvas with highest sorting order
            var canvas = ChartboostMediationUtils.GetCanvasWithHighestSortingOrder();

            // Instantiate inside this canvas
            var unityBannerAd = new GameObject("ChartboostMediationUnityBannerAd")
                .AddComponent<ChartboostMediationUnityBannerAd>();
            
            var bannerTransform = unityBannerAd.transform;
            bannerTransform.parent = canvas.transform;
            bannerTransform.localScale = Vector3.one;
            var rectTransform = unityBannerAd.gameObject.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
            
            size ??= ChartboostMediationBannerAdSize.Adaptive(BannerSize.STANDARD.Item1, BannerSize.STANDARD.Item2);
            var unityBannerAdSize = size.Name switch
            {
                "STANDARD" => UnityBannerAdSize.Standard,
                "MEDIUM" => UnityBannerAdSize.Medium,
                "LEADERBOARD" => UnityBannerAdSize.Leaderboard,
                _ => UnityBannerAdSize.Adaptive
            };
            unityBannerAd.SetUnityBannerAdSize(unityBannerAdSize);
            
            var canvasScale = canvas.transform.localScale.x;
            var width = ChartboostMediationConverters.NativeToPixels(size.Width)/canvasScale;
            var height = ChartboostMediationConverters.NativeToPixels(size.Height)/canvasScale;
            rectTransform.sizeDelta = new Vector2(width, height);

            PlaceUnityBannerAd(unityBannerAd, screenLocation);
            
            return unityBannerAd;
        }
        
        private static void PlaceUnityBannerAd(ChartboostMediationUnityBannerAd unityBannerAd,
            ChartboostMediationBannerAdScreenLocation screenLocation)
        {
            var anchor = Vector2.zero;
            var pivot = Vector2.zero;
            switch (screenLocation)
            {
                case ChartboostMediationBannerAdScreenLocation.TopLeft:
                    anchor = new Vector2(0, 1);
                    pivot = new Vector2(0, 1);
                    break;
                case ChartboostMediationBannerAdScreenLocation.TopCenter:
                    anchor = new Vector2(0.5f, 1);
                    pivot = new Vector2(0.5f, 1);
                    break;
                case ChartboostMediationBannerAdScreenLocation.TopRight:
                    anchor = new Vector2(1, 1);
                    pivot = new Vector2(1, 1);
                    break;
                case ChartboostMediationBannerAdScreenLocation.Center:
                    anchor = new Vector2(0.5f, .5f);
                    pivot = new Vector2(0.5f, 0.5f);
                    break;
                case ChartboostMediationBannerAdScreenLocation.BottomLeft:
                    anchor = new Vector2(0, 0);
                    pivot = new Vector2(0, 0);
                    break;
                case ChartboostMediationBannerAdScreenLocation.BottomCenter:
                    anchor = new Vector2(0.5f, 0);
                    pivot = new Vector2(0.5f, 0);
                    break;
                case ChartboostMediationBannerAdScreenLocation.BottomRight:
                    anchor = new Vector2(1, 0);
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
