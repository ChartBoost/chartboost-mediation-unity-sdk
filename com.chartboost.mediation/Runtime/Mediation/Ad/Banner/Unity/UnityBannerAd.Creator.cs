using Chartboost.Mediation.Utilities;
using UnityEngine;

namespace Chartboost.Mediation.Ad.Banner.Unity
{
    public partial class UnityBannerAd
    {
        private const string GameObjectDefaultName = "UnityBannerAd";
        
        internal static UnityBannerAd InstantiateUnityBannerAd(Transform parent = null, BannerSize? size = null)
        {
            parent ??= CanvasUtilities.GetCanvas().transform;
            
            // Instantiate inside this canvas
            var unityBannerAd = new GameObject(GameObjectDefaultName).AddComponent<UnityBannerAd>();
            
            var bannerTransform = unityBannerAd.transform;
            bannerTransform.SetParent(parent);
            bannerTransform.localScale = Vector3.one;
            
            return unityBannerAd;
        }
    }
}
