using UnityEngine;

public class ColorMixing
{
    public static Color Mix(Color c1 , Color c2)
    {
        var r = (c1.r + c2.r) / 2;
        var g = (c1.g + c2.g) / 2;
        var b = (c1.b + c2.b) / 2;
        return new Color(r, g, b);
    }
}
