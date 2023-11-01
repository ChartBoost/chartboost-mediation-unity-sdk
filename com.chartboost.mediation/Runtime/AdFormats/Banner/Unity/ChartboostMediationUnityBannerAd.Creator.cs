using Chartboost.Banner;
using Chartboost.Utilities;
using UnityEditor;
using UnityEngine;

namespace Chartboost.AdFormats.Banner.Unity
{
    public sealed partial class ChartboostMediationUnityBannerAd
    {
        #if UNITY_EDITOR
        private const string MenuItemUnityBannerAd = "GameObject/Chartboost Mediation/UnityBannerAd";

        [MenuItem(MenuItemUnityBannerAd)]
        public static void CreateAd()
        {
            Instantiate(Selection.activeTransform);
        }
        #endif
        
        private const string GameObjectDefaultName = "ChartboostMediationUnityBannerAd";

        internal static ChartboostMediationUnityBannerAd Instantiate(
            Transform parent = null,
            ChartboostMediationBannerSize? size = null,
            ChartboostMediationBannerAdScreenLocation screenLocation = ChartboostMediationBannerAdScreenLocation.Center, 
            bool conformToSafeArea = false)
        {
            parent ??= ChartboostMediationUtils.GetCanvas().transform;
            
            // Instantiate inside this canvas
            var unityBannerAd = new GameObject(GameObjectDefaultName)
                .AddComponent<ChartboostMediationUnityBannerAd>();
            
            var bannerTransform = unityBannerAd.transform;
            bannerTransform.SetParent(parent);
            bannerTransform.localScale = Vector3.one;

            // If no size is provided use Standard size as default
            var containerSize = size ?? ChartboostMediationBannerSize.Standard;
            unityBannerAd.SetSizeType(containerSize.SizeType);
            
            var canvasScale = parent.GetComponentInParent<Canvas>()?.transform.localScale.x ?? 1;
            var width = ChartboostMediationConverters.NativeToPixels(containerSize.Width)/canvasScale;
            var height = ChartboostMediationConverters.NativeToPixels(containerSize.Height)/canvasScale;
            
            var rectTransform = unityBannerAd.gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(width, height);
            
            PlaceUnityBannerAd(unityBannerAd, screenLocation, conformToSafeArea);
            
            return unityBannerAd;
        }

        private static readonly Vector2 TopCenterPivot = new Vector2(0.5f, 1);
        private static readonly Vector2 CenterPivot = new Vector2(0.5f, 0.5f);
        private static readonly Vector2 BottomCenter = new Vector2(0.5f, 0);

        private static void PlaceUnityBannerAd(ChartboostMediationUnityBannerAd unityBannerAd, ChartboostMediationBannerAdScreenLocation screenLocation, bool useSafeArea = false)
        {
            var left = useSafeArea ? Screen.safeArea.xMin / Screen.width : 0;
            var right = useSafeArea ? Screen.safeArea.xMax / Screen.width : 1;
            var top = useSafeArea ? Screen.safeArea.yMax / Screen.height : 1;
            var bottom = useSafeArea ? Screen.safeArea.yMin / Screen.height : 0;
            
            const float center = 0.5f;
            
            var pivot = Vector2.zero;
            var anchor = Vector2.zero;
            switch (screenLocation)
            {
                case ChartboostMediationBannerAdScreenLocation.TopLeft:
                    anchor = new Vector2(left, top);
                    pivot = Vector2.up;
                    break;
                case ChartboostMediationBannerAdScreenLocation.TopCenter:
                    anchor = new Vector2(center, top);
                    pivot = TopCenterPivot;
                    break;
                case ChartboostMediationBannerAdScreenLocation.TopRight:
                    anchor = new Vector2(right, top);
                    pivot = Vector2.one;
                    break;
                case ChartboostMediationBannerAdScreenLocation.Center:
                    anchor = new Vector2(center, center);
                    pivot = CenterPivot;
                    break;
                case ChartboostMediationBannerAdScreenLocation.BottomLeft:
                    anchor = new Vector2(left, bottom);
                    break;
                case ChartboostMediationBannerAdScreenLocation.BottomCenter:
                    anchor = new Vector2(center, bottom);
                    pivot = BottomCenter;
                    break;
                case ChartboostMediationBannerAdScreenLocation.BottomRight:
                    anchor = new Vector2(right, bottom);
                    pivot = Vector2.right;
                    break;
            }
            
            
            var rect = unityBannerAd.GetComponent<RectTransform>();

            rect.anchorMin = rect.anchorMax = anchor;
            rect.pivot = pivot;
            rect.anchoredPosition = Vector2.zero;
        }
    }
}
