using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static bool IsFacingTarget(this Transform transform, Transform target, float angle = 30)
    {
        float dotThreshold = Mathf.Cos(Mathf.Deg2Rad * angle);
        // 向量点积的特性来判断扇形区间
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();

        float dot = Vector3.Dot(transform.forward, vectorToTarget);

        // Debug.Log("dot: " + dot + " threshold: " + dotThreshold);

        return dot >= dotThreshold;
    }
}
