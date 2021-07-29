namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using UnityEditor;
    public static class CSprite_Gizmo
    {
        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected | GizmoType.Pickable)]
        static void ExampleGizmo(ADrawableBase comp, GizmoType type)
        {
            Gizmos.matrix = comp.transform.localToWorldMatrix;
            Gizmos.color = type.HasFlag(GizmoType.Selected) ? new Color(1, 1, 1, 0.1f) : new Color(0, 0, 0, 0);
            Gizmos.DrawCube(Vector3.zero, comp.LocalSize);
        }
    }
}
