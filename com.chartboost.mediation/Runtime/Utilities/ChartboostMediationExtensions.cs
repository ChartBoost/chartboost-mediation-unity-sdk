using System;
using Chartboost.AdFormats.Banner;
using Newtonsoft.Json;
using UnityEngine;
using static Chartboost.Utilities.Constants;

namespace Chartboost.Utilities
{
    [Serializable]
    public class LayoutParams
    {
        public float x;
        public float y;
        public float width;
        public float height;
        [JsonIgnore]public Vector2 topLeft;
        [JsonIgnore]public Vector2 bottomLeft;
        [JsonIgnore]public Vector2 topRight;
        [JsonIgnore]public Vector2 bottomRight;
    }
    
    public static class ChartboostMediationExtensions
    {
        public static LayoutParams LayoutParams(this RectTransform rectTransform)
        {
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            // corners[0] -> bottom-left
            // corners[1] -> top-left
            // corners[2] -> top-right
            // corners[3] -> bottom-right
            //    1           2
            //     _ _ _ _ _ _ 
            //    |           |
            //    |           |
            //    |           |
            //     - - - - - -
            //    0           3

            var lp = new LayoutParams
            {
                x = corners[0].x,  
                y = corners[1].y,
                width = (int)(corners[2].x - corners[0].x),
                height = (int)(corners[1].y - corners[0].y),
                bottomLeft = corners[0],
                topLeft = corners[1],
                topRight = corners[2],
                bottomRight =  corners[3]
            };
        
            return lp;
        }

        public static Vector2 Size(this ChartboostMediationBannerSizeType sizeType)
        {
            return sizeType switch
            {
                ChartboostMediationBannerSizeType.Standard => new Vector2(BannerSize.STANDARD.Item1, BannerSize.STANDARD.Item2),
                ChartboostMediationBannerSizeType.Medium => new Vector2(BannerSize.MEDIUM.Item1, BannerSize.MEDIUM.Item2),
                ChartboostMediationBannerSizeType.Leaderboard => new Vector2(BannerSize.LEADERBOARD.Item1, BannerSize.LEADERBOARD.Item2),
                _ => Vector2.zero
            };
        }
    }
}
