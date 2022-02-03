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
        public float[] Values;

        // Publics
        public void Invoke(Actionable user, Targetable target)
        {
            if (!TestForWarnings(target))
                return;

            Effect.Invoke(user, target, Values);
        }

        // Privates
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
    }
}

/*
else if (Values.Length<Effect.RequiredValuesCount)
    return WarningTooFewValues();
else if (Values.Length > Effect.RequiredValuesCount)
    return WarningRedundantValues();
private bool WarningTooFewValues()
{
    Debug.LogWarning($"TooFewValues:\ttrying to invoke effect {Effect.GetType().Name} with {Values.Length} values, while it requires {Effect.RequiredValuesCount}\n" +
    $"Fallback:\treturn without invoking the effect");
    return false;
}
private bool WarningRedundantValues()
{
    Debug.LogWarning($"RedundantValues:\tinvoking effect {Effect.GetType().Name} with {Values.Length - Effect.RequiredValuesCount} values too many");
    return true;
}
*/