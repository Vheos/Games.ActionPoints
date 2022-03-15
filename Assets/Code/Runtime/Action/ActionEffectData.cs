namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    // Defines
    [Serializable]
    public struct ActionEffectData
    {
        // Inspector    
        public ActionEffect Effect;
        public ActionAgent Subject;
        public ActionAgent Object;
        public float[] Values;

        // Publics
        public void Invoke(Actionable user, Targetable target, ActionStats stats)
        {
            if (Effect == null)
                return;

            ABaseComponent EnumToAgent(ActionAgent agentEnum)
            => agentEnum switch
            {
                ActionAgent.User => user,
                ActionAgent.Target => target,
                _ => default,
            };

            Effect.Invoke(EnumToAgent(Subject), EnumToAgent(Object), Values, stats);
        }
    }
}

/*
        private Type[] _cachedRequiredComponenets;
        public Type[] CachedRequiredComponents
        {
            get
            {
                if (_cachedRequiredComponenets == null)
                    _cachedRequiredComponenets = RequiredComponents;
                return _cachedRequiredComponenets;
            }
        }

       

        private bool HasComponents(Targetable target, Type[] componentTypes, out Type missingComponentType)
        {
            foreach (var type in componentTypes)
                if (target.GetComponent(type) == null)
                {
                    missingComponentType = type;
                    return false;
                }

            missingComponentType = null;
            return true;
        }
        private bool TestForWarnings(Targetable target)
        {
            if (Effect == null)
                return WarningNullEffect();
            else if (!HasComponents(target, Effect.CachedRequiredComponents, out var missingComponentType))
                return WarningMissingComponent(missingComponentType);
            return true;
        }
        private bool WarningNullEffect()
        {
            Debug.LogWarning($"NullEffect:\ttrying to invoke a null effect\n" +
            $"Fallback:\treturn without invoking the effect");
            return false;
        }
        private bool WarningMissingComponent(Type missingComponentType)
        {
            Debug.LogWarning($"MissingComponent:\ttrying to invoke effect {Effect.GetType().Name} on a target that doesn't have a {missingComponentType.Name}\n" +
            $"Fallback:\treturn without invoking the effect");
            return false;
        }
*/