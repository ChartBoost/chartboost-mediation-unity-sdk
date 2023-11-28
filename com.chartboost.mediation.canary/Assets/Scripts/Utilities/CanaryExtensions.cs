using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public static class CanaryExtensions
    {
        public static T AddOrGetComponent<T>(this GameObject gameObject) where T : Component
            => gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();

        public static void ScrollTo(this ScrollRect scrollView, RectTransform rectTransform)
        {
            // if provided rectTransform is not part of the scrollview
            if(!scrollView.GetComponentInChildren<RectTransform>(rectTransform))
                return;
        
            var parent = rectTransform.parent;
            float childCount = 0;
            for (var i = 0; i < parent.childCount; i++)
            {
                // Don't consider the elements with ignoreLayout 
                var layoutElement = parent.GetChild(i).GetComponent<LayoutElement>();
                if (layoutElement != null && layoutElement.ignoreLayout)
                    continue;
            
                childCount++;
            }
        
            var scrollValue = 1 - rectTransform.GetSiblingIndex() / childCount;

            if (scrollView.horizontal)
            {
                scrollView.horizontalNormalizedPosition = scrollValue;
                if(scrollView.horizontalScrollbar!= null)
                    scrollView.horizontalScrollbar.value = scrollValue;
            }

            if (scrollView.vertical)
            {
                scrollView.verticalNormalizedPosition = scrollValue;
                if(scrollView.verticalScrollbar!= null)
                    scrollView.verticalScrollbar.value = scrollValue;
            }
        }
    }
}
