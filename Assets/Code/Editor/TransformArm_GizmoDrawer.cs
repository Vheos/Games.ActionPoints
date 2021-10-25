#if UNITY_EDITOR
namespace Vheos.Games.ActionPoints.Editor
{
    using UnityEngine;
    using UnityEditor;
    using Tools.Extensions.UnityObjects;

    public static class TransformArm_GizmoDrawer
    {
        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected | GizmoType.Pickable)]
        static void Pickable(TransformArm comp, GizmoType type)
        {
            if (!comp.isActiveAndEnabled)
                return;

            foreach (var childTransform in comp.GetChildTransforms())
            {
                Gizmos.color = comp._Color;
                Gizmos.DrawLine(comp.transform.position, childTransform.position);
            }
        }
    }
}
#endif