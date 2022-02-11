namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using System.Collections.Generic;

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
                effectData.Invoke(user, target, ref stats);
        }
        public IReadOnlyCollection<Type> RequiredComponentTypes
        {
            get
            {
                if (_requiredComponentTypes == null)
                    InitializeRequiredComponentTypes();
                return _requiredComponentTypes;
            }
        }

        // Privates
        private HashSet<Type> _requiredComponentTypes;
        private void InitializeRequiredComponentTypes()
        {
            _requiredComponentTypes = new();
            foreach (var effectData in Effects)
                foreach (var componentType in effectData.Effect.RequiredComponentTypes)
                    _requiredComponentTypes.Add(componentType);
        }
    }
}