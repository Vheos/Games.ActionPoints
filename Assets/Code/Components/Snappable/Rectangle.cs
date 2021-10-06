namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.Math;
    public class Rectangle : ASnappable
    {
        // Overrides
        public override Vector3 GetClosestSurfacePointLocal(Vector3 point)
        => point.XY().NormalizeComps().Div(2f);

#if UNITY_EDITOR
        // Overrides
        public override void DrawGizmo()
        {
            Rect rect = new Rect
            {
                size = Vector2.one,
                center = Vector2.zero,
            };
            Editor.EditorUtility.DrawRect(rect, transform, _Color);
        }
#endif
    }
}