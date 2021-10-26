namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;

    [DisallowMultipleComponent]
    abstract public class ASnappable : ABaseComponent
    {
        // Public
        abstract public Vector3 GetClosestSurfacePointLocal(Vector3 point);
        public Vector3 GetClosestSurfacePoint(Vector3 point)
        => GetClosestSurfacePointLocal(point.Untransform(transform)).Transform(transform);

#if UNITY_EDITOR
        // Inspector
       [SerializeField]  protected Color _Color = Color.green;
        abstract public void DrawGizmo();

        // Play
private void OnDrawGizmos()
        => DrawGizmo();
#endif
    }
}