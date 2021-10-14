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
        public Vector3 SnappableOffset
        => _Snappable.transform.position.OffsetTo(transform.position - _Offset);
        public Vector3 LocalSnappableOffset
        => transform.position.Sub(_Offset).Untransform(_Snappable.transform);
        public void Snap(ASnappable snappable, float lerpAlpha)
        {
            Vector3 targetPosition = snappable.GetClosestSurfacePoint(transform.position - _Offset);
            transform.position = transform.position.Lerp(targetPosition + _Offset, lerpAlpha);
        }

        // Mono
        public override void PlayUpdate()
        {
            base.PlayUpdate();
            if (_Snappable == null)
                return;

            Snap(_Snappable, NewUtility.LerpHalfTimeToAlpha(_HalfTime));
        }

#if UNITY_EDITOR
        public override void EditUpdate()
        {
            base.EditUpdate();
            if (_Snappable == null || !_RunInEditor)
                return;

            Snap(_Snappable, 1f);
        }
#endif
    }
}