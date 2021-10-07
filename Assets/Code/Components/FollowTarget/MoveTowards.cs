namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Vheos.Tools.Extensions.Math;
    public class MoveTowards : AFollowTarget
    {
        // Overrides
        public override void Follow(Transform target, float lerpAlpha)
        => transform.position = transform.position.Lerp(target.position + _Offset, lerpAlpha);
    }
}