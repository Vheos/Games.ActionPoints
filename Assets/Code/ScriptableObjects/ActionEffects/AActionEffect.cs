namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;

    abstract public class AActionEffect : ScriptableObject
    {
        // Constants
        protected const string CONTEXT_MENU_PATH = "ActionEffects/";

        // Public
        abstract public void Invoke(ABaseComponent user, ABaseComponent target, params float[] values);

        // Private
        abstract protected int RequiredValuesCount
        { get; }

        // Defines
        [System.Serializable]
        public class Data
        {
            // Inspector     
            [SerializeField] protected AActionEffect _Effect = null;
            [SerializeField] protected Direction _Direction = Direction.FromTargetToUser;
            [SerializeField] protected float[] _Values = new float[1];

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
                $"Fallback: return without invoking the effect");
                return true;
            }
            private bool WarningTooFewValues(Type type, int valuesCount, int requiredValuesCount)
            {
                Debug.LogWarning($"TooFewValues:\ttrying to invoke effect {type.Name} with {valuesCount} values, while it requires {requiredValuesCount}\n" +
                $"Fallback: return without invoking the effect");
                return true;
            }
            private bool WarningRedundantValues(Type type, int redundantValuesCount)
            {
                Debug.LogWarning($"RedundantValues:\tinvoking effect {type.Name} with {redundantValuesCount} values too many");
                return false;
            }

            // Publics
            public void Invoke(Actionable user, ABaseComponent target)
            {
                if (TestForWarnings())
                    return;

                switch (_Direction)
                {
                    case Direction.FromUserToTarget: _Effect.Invoke(user, target, _Values); break;
                    case Direction.FromTargetToUser: _Effect.Invoke(target, user, _Values); break;
                }
            }

            // Defines
            protected enum Direction
            {
                FromUserToTarget,
                FromTargetToUser,
            }
        }
    }
}