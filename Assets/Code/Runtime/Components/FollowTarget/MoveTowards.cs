namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;

    public class MoveTowards : AFollowTarget
    {
        // Publics
        public Vector3 FinalPosition
        {
            get
            {
                Vector3 targetPosition = TargetVector;
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
        }

        // Overrides
        protected override Vector3 TargetVector
        => _useTransform ? _Transform.position : _Vector;
        protected override void FollowOnUpdate()
        => transform.position = transform.position.Lerp(FinalPosition, NewUtility.LerpHalfTimeToAlpha(_HalfTime));
        protected override void FollowOnAnimate()
        => this.NewTween()
            .SetDuration(_AnimDuration)
            .Position(FinalPosition);
    }
}