namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.Math;

    [System.Serializable]
    public class QAnimFloat : AQurveAnimation<float>
    {
        public override float Value
        => _from.Lerp(__To, QurveValue);
    }

    [System.Serializable]
    public class QAnimVector2 : AQurveAnimation<Vector2>
    {
        public override Vector2 Value
        => _from.Lerp(__To, QurveValue);
    }

    [System.Serializable]
    public class QAnimVector3 : AQurveAnimation<Vector3>
    {
        public override Vector3 Value
        => _from.Lerp(__To, QurveValue);
    }

    [System.Serializable]
    public class QAnimQuaternion : AQurveAnimation<Quaternion>
    {
        public override Quaternion Value
        => Quaternion.SlerpUnclamped(_from, __To, QurveValue);
    }

    [System.Serializable]
    public class QAnimColor : AQurveAnimation<Color>
    {
        public override Color Value
        => Color.LerpUnclamped(_from, __To, QurveValue);
    }

}