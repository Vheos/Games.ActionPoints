namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;

    abstract public class AFollowTargetRotation : AFollowTarget
    {
        // Publics
        public Quaternion GetFinalRotation(Transform target)
        {
            Vector3 targetAngles = TargetAngles(target);
            if (_LockedAxes != 0)
            {
                Vector3 currentAngles = transform.rotation.eulerAngles;
                if (_LockedAxes.HasFlag(Axes.X))
                    targetAngles.x = currentAngles.x;
                if (_LockedAxes.HasFlag(Axes.Y))
                    targetAngles.y = currentAngles.y;
                if (_LockedAxes.HasFlag(Axes.Z))
                    targetAngles.z = currentAngles.z;
            }
            return Quaternion.Euler(targetAngles + _Offset);
        }

        // Privates
        abstract protected Vector3 TargetAngles(Transform target);

        // Overrides
        protected override void UpdateFollow()
        => transform.rotation = transform.rotation.Lerp(GetFinalRotation(_Target), NewUtility.LerpHalfTimeToAlpha(_HalfTime));
        protected override void FollowOnAnimate(System.Action tryRestoreEnabled)
        => transform.AnimateRotation(this, GetFinalRotation(_Target), _AnimDuration, tryRestoreEnabled);
    }
}