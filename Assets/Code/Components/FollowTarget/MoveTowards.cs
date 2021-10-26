namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.Math;
    public class MoveTowards : AFollowTarget
    {
        // Overrides
        public override void Follow(Transform target, float lerpAlpha)
        {
            Vector3 targetPosition = target.position + _Offset;
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
            transform.position = transform.position.Lerp(targetPosition, lerpAlpha);
        }
    }
}