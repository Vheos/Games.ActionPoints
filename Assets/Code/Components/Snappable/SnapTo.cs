namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;

    public class SnapTo : AUpdatable
    {
        // Inspector
        public bool _RunInEditor = true;
        public ASnappable _Snappable = null;
        public Vector3 _Offset = Vector3.zero;
        [Range(0f, 1f)] public float _HalfTime = 0.1f;

        // Public
        public bool IsActive
        => isActiveAndEnabled && _Snappable != null;
        public Vector3 TargetPosition
        => _Snappable.GetClosestSurfacePoint(transform.position - _Offset) + _Offset;
        public void Snap(float lerpAlpha)
        => transform.position = transform.position.Lerp(TargetPosition, lerpAlpha);

        // Mono
        public override void PlayUpdate()
        {
            base.PlayUpdate();
            if (_Snappable == null)
                return;

            Snap(NewUtility.LerpHalfTimeToAlpha(_HalfTime));
        }

#if UNITY_EDITOR
        public override void EditUpdate()
        {
            base.EditUpdate();
            if (_Snappable == null || !_RunInEditor)
                return;

            Snap(1f);
        }
#endif
    }
}