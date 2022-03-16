namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;
    using Games.Core;
    using Tools.Utilities;

    [CreateAssetMenu(fileName = nameof(Action), menuName = ASSET_MENU_PATH + nameof(Action))]
    public class Action : ScriptableObject
    {
        // Constants
        public const string ASSET_MENU_PATH = "";

        // Inspector
        [field: SerializeField,] public ActionButtonVisuals ButtonVisuals { get; private set; }
        [field: SerializeField] public ActionPhase Phase { get; private set; }
        [field: SerializeField] public ActionExecution Execution { get; private set; }
        [field: SerializeField, Range(0, 5)] public int ActionPointsCost { get; private set; }
        [field: SerializeField, Range(0, 5)] public int FocusPointsCost { get; private set; }
        [field: SerializeField] public ActionEffectData[] Effects { get; private set; }

        // Publics (use)
        public void Use(Actionable user, Targetable target)
        {
            user.ActionProgress -= ActionPointsCost;
            user.FocusProgress -= FocusPointsCost;

            ActionStats stats = new();
            foreach (var effectData in Effects)
                effectData.Invoke(user, target, stats);
        }
        public IReadOnlyDictionary<ActionAgent, HashSet<Type>> RequiredComponents
        {
            get
            {
                if (_cachedRequiredComponentsByAgent == null)
                    InitializeRequiredComponents();
                return _cachedRequiredComponentsByAgent;
            }
        }
        public bool CheckUserComponents(Actionable user)
        {
            foreach (var requiredComponentType in RequiredComponents[ActionAgent.User])
                if (!user.Has(requiredComponentType))
                    return false;
            return true;
        }
        public bool CheckTargetComponents(Targetable target)
        {
            foreach (var requiredComponentType in RequiredComponents[ActionAgent.Target])
                if (!target.Has(requiredComponentType))
                    return false;
            return true;
        }
        public bool CheckConditions(Actionable user, Targetable target)
        {
            // Eat
            // - IsSelf
            // - HasAnyWound

            // Attack, StartCombat
            // - IsEnemy

            // LeaveCombat
            // - IsSelf

            return true;
        }

        // Privates
        private Dictionary<ActionAgent, HashSet<Type>> _cachedRequiredComponentsByAgent;
        private void InitializeRequiredComponents()
        {
            _cachedRequiredComponentsByAgent = new();
            foreach (var target in Utility.GetEnumValues<ActionAgent>())
                _cachedRequiredComponentsByAgent[target] = new();

            foreach (var effectData in Effects)
            {
                foreach (var requiredComponent in effectData.Effect.SubjectRequiredComponents)
                    _cachedRequiredComponentsByAgent[effectData.Subject].Add(requiredComponent);
                foreach (var requiredComponent in effectData.Effect.ObjectRequiredComponents)
                    _cachedRequiredComponentsByAgent[effectData.Object].Add(requiredComponent);
            }
        }
    }
}