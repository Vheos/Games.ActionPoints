namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.Math;
    public class Oval : ASnappable
    {
        // Overrides
        public override Vector3 GetClosestSurfacePointLocal(Vector3 point)
        => point.XY().normalized.Div(2f);

#if UNITY_EDITOR
        // Inspector
        [Range(3, 60)] public int _Vertices = 12;

        // Overrides
        public override void DrawGizmo()
        => Editor.EditorUtility.DrawCircle(Vector3.zero, 0.5f, transform, _Color, _Vertices);
#endif
    }
}