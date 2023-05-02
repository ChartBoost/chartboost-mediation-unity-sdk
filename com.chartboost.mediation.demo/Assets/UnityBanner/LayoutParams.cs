using System;

[Serializable]
public class LayoutParams
{
    public float x;
    public float y;
    public int width;
    public int height;

    public LayoutParams() { }

    public LayoutParams(float x, float y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }
}
