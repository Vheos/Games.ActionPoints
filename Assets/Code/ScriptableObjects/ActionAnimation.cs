namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.Extensions.General;

    [CreateAssetMenu(fileName = nameof(ActionAnimation), menuName = nameof(ActionAnimation), order = 3)]
    public class ActionAnimation : ScriptableObject
    {
        // Inspector
        public StateData Charge;
        public StateData Release;

        // Defines
        [System.Serializable]
        public struct StateData
        {
            // Inspector
            [Range(0f, 1f)] public float _Duration;
            [Range(0f, 1f)] public float _WaitTime;
            [Range(-0.5f, +0.5f)] public float _ForwardDistance;
            [Range(0f, 1f)] public float _ArmLength;
            public Vector3 _ArmRotation;
            public Vector3 _HandRotation;
            public bool _ForwardDistanceEnabled;
            public bool _ArmLengthEnabled;
            public bool _ArmRotationEnabled;
            public bool _HandRotationEnabled;
        }
    }
}