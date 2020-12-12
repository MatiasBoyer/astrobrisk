using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{

    public static Vector2 screenRefRes = new Vector2(1080, 1920);

    public static float Remap(float from, float fromMin, float fromMax, float toMin, float toMax)
    {
        var fromAbs = from - fromMin;
        var fromMaxAbs = fromMax - fromMin;

        var normal = fromAbs / fromMaxAbs;

        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;

        var to = toAbs + toMin;

        return to;
    }

    public static Vector2 WorldToCanvas(RectTransform canvasrect, Vector3 point)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasrect, point, Camera.main, out pos);

        return pos;
    }

}
