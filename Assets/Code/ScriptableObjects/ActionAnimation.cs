namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.Extensions.General;

    [CreateAssetMenu(fileName = nameof(ActionAnimation), menuName = nameof(ActionAnimation), order = 3)]
    public class ActionAnimation : ScriptableObject
    {
        // Inspector
        [SerializeField] protected Clip[] _Charge = new Clip[1];
        [SerializeField] protected Clip[] _Cancel = new Clip[1];
        [SerializeField] protected Clip[] _Release = new Clip[1];

        // Public
        public Clip[] Charge
        => _Charge;
        public Clip[] Cancel
        => _Cancel;
        public Clip[] Release
        => _Release;

        // Defines
        [System.Serializable]
        public class Clip
        {
            // Inspector
            [Range(0f, 1f)] public float Duration = 0.5f;
            [Range(0f, 1f)] public float StayUpTime = 0f;
            public QAnimator.Curve Style = QAnimator.Curve.Qurve;
            public VisibleParameters Parameters;

            public Vector3 ArmRotation = Vector3.zero;
            [Range(0f, 1f)] public float ArmLength = 0f;
            public Vector3 HandRotation = Vector3.zero;
            [Range(0f, 2f)] public float HandScale = 1f;
            [Range(-0.5f, +0.5f)] public float ForwardDistance = 0f;

            public bool ForwardDistanceUseIdle = false;
            public bool ArmLengthUseIdle = false;
            public bool ArmRotationUseIdle = false;
            public bool HandRotationUseIdle = false;
            public bool HandScaleUseIdle = false;

            // Publics
            public float TotalTime
            => Duration + StayUpTime;
            public bool ForwardDistanceEnabled
            => Parameters.HasFlag(VisibleParameters.ForwardDistance);
            public bool ArmLengthEnabled
             => Parameters.HasFlag(VisibleParameters.ArmLength);
            public bool ArmRotationEnabled
            => Parameters.HasFlag(VisibleParameters.ArmRotation);
            public bool HandRotationEnabled
            => Parameters.HasFlag(VisibleParameters.HandRotation);
            public bool HandScaleEnabled
            => Parameters.HasFlag(VisibleParameters.HandScale);

            public Vector3 GetArmRotation(Clip idle)
            => ArmRotationUseIdle ? idle.ArmRotation : ArmRotation;
            public float GetArmLength(Clip idle)
            => ArmLengthUseIdle ? idle.ArmLength : ArmLength;
            public float GetHandScale(Clip idle)
            => HandScaleUseIdle ? idle.HandScale : HandScale;
            public Vector3 GetHandRotation(Clip idle)
            => HandRotationUseIdle ? idle.HandRotation : HandRotation;
            public float GetForwardDistance(Clip idle)
            => ForwardDistanceUseIdle ? idle.ForwardDistance : ForwardDistance;

            // Defines
            [System.Flags]
            public enum VisibleParameters
            {
                ArmRotation = 1 << 2,
                ArmLength = 1 << 1,
                HandRotation = 1 << 3,
                HandScale = 1 << 4,
                ForwardDistance = 1 << 0,
            }
        }
    }
}