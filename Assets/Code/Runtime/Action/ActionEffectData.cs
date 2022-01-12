namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;

    // Defines
    [Serializable]
    public class ActionEffectData
    {
        // Inspector    
        [SerializeField] protected ActionEffect _Effect = null;
        [SerializeField] protected float[] _Values = new float[1];

        // Publics
        public void Invoke(Actionable user, Targetable target)
        {
            if (TestForWarnings())
                return;
            _Effect.Invoke(user, target, _Values);

        }

        // Privates
        private bool TestForWarnings()
        {
            if (_Effect == null)
                return WarningNullEffect();
            else if (_Values.Length < _Effect.RequiredValuesCount)
                return WarningTooFewValues(_Effect.GetType(), _Values.Length, _Effect.RequiredValuesCount);
            else if (_Values.Length > _Effect.RequiredValuesCount)
                return WarningRedundantValues(_Effect.GetType(), _Values.Length - _Effect.RequiredValuesCount);
            return false;
        }
        private bool WarningNullEffect()
        {
            Debug.LogWarning($"NullEffect:\ttrying to invoke a null effect\n" +
            $"Fallback:\treturn without invoking the effect");
            return true;
        }
        private bool WarningTooFewValues(Type type, int valuesCount, int requiredValuesCount)
        {
            Debug.LogWarning($"TooFewValues:\ttrying to invoke effect {type.Name} with {valuesCount} values, while it requires {requiredValuesCount}\n" +
            $"Fallback:\treturn without invoking the effect");
            return true;
        }
        private bool WarningRedundantValues(Type type, int redundantValuesCount)
        {
            Debug.LogWarning($"RedundantValues:\tinvoking effect {type.Name} with {redundantValuesCount} values too many");
            return false;
        }

    }
}