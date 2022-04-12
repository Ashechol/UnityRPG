using UnityEngine;

namespace Utils
{
    public class GizmosEx
    {
        public static void DrawWireArc(Transform origin, float radius, float angle, Color color,
                                       int segments = 50, float deviation = 0)
        {
            Gizmos.color = color;
            Vector3 offset = Vector3.up * deviation;
            Vector3 dir = Quaternion.AngleAxis(angle, origin.up) * origin.forward * radius + offset;

            float mdeg = angle * 2 / segments;
            Vector3 currentPos = origin.position + dir;
            Vector3 oldPos;

            if (angle != 180)
                Gizmos.DrawLine(origin.position + offset, currentPos);

            for (int i = 0; i <= segments; i++)
            {
                oldPos = currentPos;
                currentPos = origin.position + Quaternion.AngleAxis(-i * mdeg, origin.up) * dir;
                Gizmos.DrawLine(oldPos, currentPos);
            }

            if (angle != 180)
                Gizmos.DrawLine(origin.position + offset, currentPos);

        }
    }
}