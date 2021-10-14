namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.Math;
    public class RotateAs : AFollowTarget
    {
        // Overrides
        public override void Follow(Transform target, float lerpAlpha)
        {
            Vector3 targetAngles = target.rotation.eulerAngles + _Offset;
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
            transform.rotation = transform.rotation.Lerp(Quaternion.Euler(targetAngles), lerpAlpha);
        }
    }
}