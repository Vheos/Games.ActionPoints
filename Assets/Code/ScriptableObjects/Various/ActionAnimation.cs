namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;

    [CreateAssetMenu(fileName = nameof(ActionAnimation), menuName = nameof(ActionAnimation), order = 3)]
    public class ActionAnimation : ScriptableObject
    {
        // Inspector
        [SerializeField] protected Clip[] _Charge = new Clip[1];
        [SerializeField] protected Clip[] _Cancel = new Clip[1];
        [SerializeField] protected Clip[] _Release = new Clip[1];

        // Public
        public IReadOnlyList<Clip> Charge
        => _Charge;
        public IReadOnlyList<Clip> Cancel
        => _Cancel;
        public IReadOnlyList<Clip> Release
        => _Release;

        // Defines
        public enum Type
        {
            Idle,
            Charge,
            Release,
        }

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
            public LookAtTarget LookAt = LookAtTarget.EnemyMidpoint;

            public bool ArmLengthUseIdle = false;
            public bool ArmRotationUseIdle = false;
            public bool HandRotationUseIdle = false;
            public bool HandScaleUseIdle = false;
            public bool ForwardDistanceUseIdle = false;
            public bool LookAtUseIdle = false;

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
            public bool LookAtEnabled
            => Parameters.HasFlag(VisibleParameters.LookAt);

            public Vector3 ChooseArmRotation(Clip idle)
            => ArmRotationUseIdle ? idle.ArmRotation : ArmRotation;
            public float ChooseArmLength(Clip idle)
            => ArmLengthUseIdle ? idle.ArmLength : ArmLength;
            public float ChooseHandScale(Clip idle)
            => HandScaleUseIdle ? idle.HandScale : HandScale;
            public Vector3 ChooseHandRotation(Clip idle)
            => HandRotationUseIdle ? idle.HandRotation : HandRotation;
            public float ChooseForwardDistance(Clip idle)
            => ForwardDistanceUseIdle ? idle.ForwardDistance : ForwardDistance;
            public LookAtTarget ChooseLookAt(Clip idle)
            => LookAtUseIdle ? idle.LookAt : LookAt;

            // Defines
            [System.Flags]
            public enum VisibleParameters
            {
                ArmRotation = 1 << 2,
                ArmLength = 1 << 1,
                HandRotation = 1 << 3,
                HandScale = 1 << 4,
                ForwardDistance = 1 << 0,
                LookAt = 1 << 5,
            }
            public enum LookAtTarget
            {
                AllyMidpoint,
                EnemyMidpoint,
                CombatMidpoint,
            }
        }
    }

    static public class ActionAnimation_Extensions
    {
        static public IReadOnlyList<ActionAnimation.Clip> ToClips(this ActionAnimation animation, ActionAnimation.Type type)
        => type switch
        {
            ActionAnimation.Type.Idle => animation.Cancel,
            ActionAnimation.Type.Charge => animation.Charge,
            ActionAnimation.Type.Release => animation.Release,
            _ => null,
        };
    }
}