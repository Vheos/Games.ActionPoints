namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;

    public class MoveTowards : AFollowTarget
    {
        // Publics
        public Vector3 GetFinalPosition(Transform target)
        {
            Vector3 targetPosition = target.position;
            if (_LockedAxes != 0)
            {
                Vector3 currentPosition = transform.position;
                if (_LockedAxes.HasFlag(Axes.X))
                    targetPosition.x = currentPosition.x;
                if (_LockedAxes.HasFlag(Axes.Y))
                    targetPosition.y = currentPosition.y;
                if (_LockedAxes.HasFlag(Axes.Z))
                    targetPosition.z = currentPosition.z;
            }
            return targetPosition + _Offset;
        }

        // Overrides
        protected override void UpdateFollow()
        => transform.position = transform.position.Lerp(GetFinalPosition(_Target), NewUtility.LerpHalfTimeToAlpha(_HalfTime));
        protected override void FollowOnAnimate(System.Action tryRestoreEnabled)
        => transform.AnimatePosition(this, GetFinalPosition(_Target), _AnimDuration, tryRestoreEnabled);
    }
}