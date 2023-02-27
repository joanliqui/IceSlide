using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyMaths
{
   public static float CalculateAngle2Points(Vector3 p1, Vector3 p2)
    {
        Vector3 v = p2 - p1;
        v.Normalize();

        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        return angle;
    }

    public static Vector2 DirFromAngle(float angleInDegrees)
    {
        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public static Vector3 CalculateDirectionVectorNormalized(Vector3 pos1, Vector3 pos2)
    {
        Vector3 dir = pos2 - pos1;
        return dir.normalized; 
    }

    public static float CalculatePercentage(float bigNumber, float smallNumber)
    {
        return (smallNumber / bigNumber) * 100;
    }
}
