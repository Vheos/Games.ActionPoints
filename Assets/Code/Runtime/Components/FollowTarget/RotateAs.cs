namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;

    public class RotateAs : AFollowTargetRotation
    {
        // Overrides
        protected override Vector3 TargetVector
        => _useTransform ? _Transform.rotation.eulerAngles : _Vector;
    }
}