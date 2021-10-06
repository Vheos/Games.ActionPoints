namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;

    [DisallowMultipleComponent]
    abstract public class ASnappable : AUpdatable
    {
        // Public
        abstract public Vector3 GetClosestSurfacePointLocal(Vector3 point);
        public Vector3 GetClosestSurfacePoint(Vector3 point)
        => GetClosestSurfacePointLocal(point.Untransform(transform)).Transform(transform);

#if UNITY_EDITOR
        // Inspector
        public Color _Color = Color.green;
        abstract public void DrawGizmo();

        // Mono
        private void OnDrawGizmos()
        => DrawGizmo();
#endif
    }
}