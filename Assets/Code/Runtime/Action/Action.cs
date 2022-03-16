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
        [field: SerializeField, Range(0, 5)] public int ActionPointsCost { get; private set; }
        [field: SerializeField, Range(0, 5)] public int FocusPointsCost { get; private set; }
        [field: SerializeField] public ActionConditionData[] Conditions { get; private set; }
        [field: SerializeField] public ActionEffectData[] Effects { get; private set; }

        // Publics (use)
        public bool IsUsableBy(Actionable user)
        => CheckPhase(user)
        && CheckResources(user)
        && CheckUserComponents(user)
        && TargetableManager.GetValidTargets(user, this).Any();
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
        public bool CheckPhase(Actionable user)
        => Phase switch
        {
            ActionPhase.Combat => user.IsInCombat(),
            ActionPhase.Camp => !user.IsInCombat(),
            _ => false,
        };
        public bool CheckResources(Actionable user)
        => !user.IsExhausted
        && user.UsableActionPoints >= ActionPointsCost
        && user.FocusPoints >= FocusPointsCost;
        public bool CheckUserComponents(Actionable user)
        {
            foreach (var requiredComponent in RequiredComponents[ActionAgent.User])
                if (!user.Has(requiredComponent))
                    return false;
            return true;
        }
        public bool CheckTargetComponents(Targetable target)
        {
            foreach (var requiredComponent in RequiredComponents[ActionAgent.Target])
                if (!target.Has(requiredComponent))
                    return false;
            return true;
        }
        public bool CheckConditions(Actionable user, Targetable target)
        {
            foreach (var condition in Conditions)
                if (!condition.Check(user, target))
                    return false;
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