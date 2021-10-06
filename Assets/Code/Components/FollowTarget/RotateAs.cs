namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Vheos.Tools.Extensions.Math;
    public class RotateAs : AFollowTarget
    {
        public override void Follow(Transform target, float lerpAlpha)
        => transform.rotation = transform.rotation.Lerp(target.rotation, lerpAlpha);
    }
}