namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;

    public class RotateTowards : AFollowTargetRotation
    {
        // Overrides
        protected override Vector3 TargetAngles(Transform target)
        => Quaternion.LookRotation(this.DirectionTowards(target)).eulerAngles;
    }
}