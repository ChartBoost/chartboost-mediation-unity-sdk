using System.Linq;
using UnityEngine;

namespace Chartboost.Utilities
{
    public static class ChartboostMediationUtils
    {
        public static Canvas GetCanvasWithHighestSortingOrder()
        {
            Canvas canvas = null;
            foreach (var can in GameObject.FindObjectsOfType<Canvas>().OrderByDescending(x => x.sortingOrder))
            {
                // Make sure the canvas is not within another canvas
                canvas = can;
                if (!can.GetComponentInParent<Canvas>())
                    break;
            }

            return canvas;
        }
    }
}
