using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethod
{
    private static float dotThreshold = 0.5f;

    public static bool IsFacingTarget(this Transform transform, Transform target)
    {
        // 向量点积的特性来判断扇形区间
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();

        float dot = Vector3.Dot(transform.forward, vectorToTarget);

        return dot >= dotThreshold;
    }

    public static void DrawWireSemicircle(this Gizmos gizmos, Transform origin, float radius, float angle)
    {
        Vector3 leftSide = Quaternion.AngleAxis(-angle, Vector3.up) * origin.forward;
        Vector3 rightSide = Quaternion.AngleAxis(angle, Vector3.up) * origin.forward;

        Vector3 currentP = leftSide * radius;
        Vector3 oldP;

        // if (angle != 180) Gizmos.DrawLine(origin.position, currentP);

        for (int i = 0; i < angle / 10; i++)
        {
            oldP = currentP;
            currentP = Quaternion.AngleAxis(10f, Vector3.up) * currentP;
            Gizmos.DrawLine(oldP, currentP);
        }
    }
}
