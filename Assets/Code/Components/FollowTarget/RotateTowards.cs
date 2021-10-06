namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Vheos.Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    public class RotateTowards : AFollowTarget
    {
        public override void Follow(Transform target, float lerpAlpha)
        => transform.rotation = transform.rotation.Lerp(Quaternion.LookRotation(this.DirectionTowards(target)), lerpAlpha);
    }
}