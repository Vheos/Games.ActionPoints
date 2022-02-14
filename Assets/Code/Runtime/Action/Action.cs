namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using System.Collections.Generic;
    using Vheos.Tools.Utilities;

    [CreateAssetMenu(fileName = nameof(Action), menuName = CONTEXT_MENU_PATH + nameof(Action))]
    public class Action : ScriptableObject
    {
        // Constants
        protected const string CONTEXT_MENU_PATH = "";

        // Inspector
        [field: SerializeField] public ActionButtonVisuals ButtonVisuals { get; private set; }
        [field: SerializeField, Range(0, 5)] public int ActionPointsCost { get; private set; }
        [field: SerializeField, Range(0, 5)] public int FocusPointsCost { get; private set; }
        [field: SerializeField] public ActionEffectData[] Effects { get; private set; }

        // Publics (use)
        public void Use(ActionTargeter user, ActionTargetable target)
        {
            user.Get<Actionable>().ActionProgress -= ActionPointsCost;
            user.Get<Actionable>().FocusProgress -= FocusPointsCost;

            ActionStats stats = new();
            foreach (var effectData in Effects)
                effectData.Invoke(user, target, stats);


        }
        public IReadOnlyDictionary<ActionTarget, HashSet<Type>> RequiredComponentTypes
        {
            get
            {
                if (_cachedRequiredComponentTypesByTarget == null)
                    InitializeRequiredComponentTypes();
                return _cachedRequiredComponentTypesByTarget;
            }
        }

        // Privates
        private Dictionary<ActionTarget, HashSet<Type>> _cachedRequiredComponentTypesByTarget;
        private void InitializeRequiredComponentTypes()
        {
            _cachedRequiredComponentTypesByTarget = new();
            foreach (var target in Utility.GetEnumValues<ActionTarget>())
                _cachedRequiredComponentTypesByTarget[target] = new();

            foreach (var effectData in Effects)
                foreach (var componentType in effectData.Effect.RequiredComponentTypes)
                    _cachedRequiredComponentTypesByTarget[effectData.Target].Add(componentType);
        }
    }
}