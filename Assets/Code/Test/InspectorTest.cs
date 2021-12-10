namespace Vheos.Games.ActionPoints.Test
{
    using System.Collections.Generic;
    using Tools.UnityCore;
    using UnityEngine;
    using UnityEngine.Events;

    public class InspectorTest : ABaseComponent
    {
        public Dictionary<string, float> _FloatsByString;
        public MonoBehaviour _MonoBehaviour;
        public UnityEvent<Character, Character, float[]> Generic;
        public AnimationCurve _AnimationCurve;
        public Gradient _Gradient;
        [ReadOnly] public string _ReadOnly = "helo";
        public ABaseComponent _script;
        public ScriptableObjectTest _scriptableObjectTest;
        public AActionTargetTest.Data _actionTargetTest;
    }
}