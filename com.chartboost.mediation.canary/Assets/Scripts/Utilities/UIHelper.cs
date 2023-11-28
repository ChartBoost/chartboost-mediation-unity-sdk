using System;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// A helper class that holds references to UI related gameobjects that may be required during runtime
    /// </summary>
    public class UIHelper : SimpleSingleton<UIHelper>
    {
        [SerializeField]
        private GameObject holder;

        /// <summary>
        /// The main canvas for this app
        /// </summary>
        public GameObject MainCanvas => gameObject;
        
        /// <summary>
        /// The gameobject that holds all the UI for this app
        /// </summary>
        public GameObject Holder => holder;

        /// <summary>
        /// The container gameobject that loads sticky banner 
        /// </summary>
        public GameObject stickyBannerContainer;
        
        /// <summary>
        /// The container gameobject that updates it's height based on the bannerView and updates it's
        /// sibling position based on the screen location selected in BannerController  
        /// </summary>
        public GameObject screenLocationBannerContainer;
        
        /// <summary>
        /// The container gameobject that updates it's height based on the sticky bannerView and updates it's
        /// sibling position based on the screen location selected in BannerController 
        /// </summary>
        public GameObject screenLocationStickyBannerContainer;

        private void Awake()
        {
            screenLocationBannerContainer = CreateBannerContainerGameObject("ScreenLocationBannerContainer");
            screenLocationStickyBannerContainer = CreateBannerContainerGameObject("ScreenLocationStickyBannerContainer");
        }

        private GameObject CreateBannerContainerGameObject(string containerName)
        {
            var container = new GameObject(containerName).AddComponent<RectTransform>().gameObject;
            container.transform.SetParent(Holder.transform);
            container.transform.localScale = Vector3.one;

            return container;
        }

    }
}
