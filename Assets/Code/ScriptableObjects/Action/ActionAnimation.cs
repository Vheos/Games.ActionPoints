namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;

    [CreateAssetMenu(fileName = nameof(ActionAnimation), menuName = CONTEXT_MENU_PATH + nameof(ActionAnimation))]
    public class ActionAnimation : ScriptableObject
    {
        // Constants
        protected const string CONTEXT_MENU_PATH = "Action/";

        // Inspector
        [SerializeField] protected Clip[] _Target = new Clip[1];
        [SerializeField] protected Clip[] _Use = new Clip[1];

        // Public
        public IReadOnlyList<Clip> Target
        => _Target;
        public IReadOnlyList<Clip> Use
        => _Use;

        // Defines
        public enum Type
        {
            Target,
            Use,
            Idle,
            UseThenIdle,
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
            [Range(-1f, 1f)] public float ArmLength = 0f;
            public Vector3 HandRotation = Vector3.zero;
            [Range(0f, 2f)] public float HandScale = 1f;
            public LookAtTarget LookAt = LookAtTarget.EnemyMidpoint;
            [Range(-1f, +1f)] public float ForwardDistance = 0f;

            public bool ArmLengthUseIdle = false;
            public bool ArmRotationUseIdle = false;
            public bool HandRotationUseIdle = false;
            public bool HandScaleUseIdle = false;
            public bool LookAtUseIdle = false;
            public bool ForwardDistanceUseIdle = false;

            // Publics
            public float TotalTime
            => Duration + StayUpTime;
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
            public bool ForwardDistanceEnabled
            => Parameters.HasFlag(VisibleParameters.ForwardDistance);

            public Vector3 ChooseArmRotation(Clip idle)
            => ArmRotationUseIdle ? idle.ArmRotation : ArmRotation;
            public float ChooseArmLength(Clip idle)
            => ArmLengthUseIdle ? idle.ArmLength : ArmLength;
            public float ChooseHandScale(Clip idle)
            => HandScaleUseIdle ? idle.HandScale : HandScale;
            public Vector3 ChooseHandRotation(Clip idle)
            => HandRotationUseIdle ? idle.HandRotation : HandRotation;
            public LookAtTarget ChooseLookAt(Clip idle)
            => LookAtUseIdle ? idle.LookAt : LookAt;
            public float ChooseForwardDistance(Clip idle)
            => ForwardDistanceUseIdle ? idle.ForwardDistance : ForwardDistance;

            // Defines
            [System.Flags]
            public enum VisibleParameters
            {
                ArmRotation = 1 << 1,
                ArmLength = 1 << 2,
                HandRotation = 1 << 3,
                HandScale = 1 << 4,
                LookAt = 1 << 5,
                ForwardDistance = 1 << 6,
            }
            public enum LookAtTarget
            {
                AllyMidpoint = 0,
                EnemyMidpoint,
                CombatMidpoint,
            }
        }
    }
}