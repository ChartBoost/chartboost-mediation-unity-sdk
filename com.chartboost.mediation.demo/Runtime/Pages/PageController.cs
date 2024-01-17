using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Chartboost.Mediation.Demo.Pages
{
    public enum PageType
    {
        Initialization,
        AdFormats,
        Placement
    }

    [Serializable]
    public struct PageData
    {
        public string name;
        public GameObject prefab;
        public PageType type;
    }

    public class PageController : MonoBehaviour
    {
        public static PageController Instance;

        public RectTransform Root => root;
        
        [SerializeField] private RectTransform root;
        [SerializeField] private List<PageData> pages;

        private static PageData? _currentPageData;

        private static readonly Dictionary<PageType, GameObject> PageInstances = new Dictionary<PageType, GameObject>();

        private void Awake()
        {
            Instance = this;
            _currentPageData = pages.Find(x => x.type == PageType.Initialization);
            var instance = GameObject.Instantiate(_currentPageData?.prefab, root);
            PageInstances.Add(PageType.Initialization, instance);
        }

        public static GameObject MoveToPage(PageType type)
        {
            var currentPageType = _currentPageData?.type ?? PageType.Initialization;
            if (_currentPageData == null || _currentPageData?.type == type)
                return null;

            if (PageInstances.TryGetValue(currentPageType, out var currentInstance))
            {
                currentInstance.SetActive(false);
            }

            _currentPageData = Instance.pages.Find(x => x.type == type);
            
            if (PageInstances.TryGetValue(type, out var nextPageInstance))
            {
                nextPageInstance.SetActive(true);
                return nextPageInstance;
            }
            
            var instance = GameObject.Instantiate(_currentPageData?.prefab, Instance.root);
            PageInstances.Add(type, instance);
            instance.SetActive(true);
            return instance;
        }
    }
}
