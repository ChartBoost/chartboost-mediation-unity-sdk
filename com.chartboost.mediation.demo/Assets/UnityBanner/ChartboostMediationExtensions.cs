using UnityEngine;

public static class ChartboostMediationExtensions
{
    public static LayoutParams LayoutParams(this RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
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

        LayoutParams lp = new LayoutParams
        {
            x = corners[0].x,   
            y = corners[1].y,   
            width = (int)(corners[2].x - corners[0].x),
            height = (int)(corners[1].y - corners[0].y)
        };

        return lp;
    }
}