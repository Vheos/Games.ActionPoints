namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;

    public class SnapTo : ABaseComponent
    {
        // Inspector
       [SerializeField]  protected bool _RunInEditor = true;
       [SerializeField]  protected ASnappable _Snappable = null;
       [SerializeField]  protected Vector3 _Offset = Vector3.zero;
        [SerializeField] [Range(0f, 1f)] protected float _HalfTime = 0.1f;

        // Public
        public bool IsActive
        => isActiveAndEnabled && _Snappable != null;
        public Vector3 TargetPosition
        => _Snappable.GetClosestSurfacePoint(transform.position - _Offset) + _Offset;
        public void Snap(float lerpAlpha)
        => transform.position = transform.position.Lerp(TargetPosition, lerpAlpha);
        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            OnPlayUpdate += () =>
            {
                if (_Snappable == null)
                    return;

                Snap(NewUtility.LerpHalfTimeToAlpha(_HalfTime));
            };
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