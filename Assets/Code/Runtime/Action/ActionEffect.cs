namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    abstract public class ActionEffect : ScriptableObject
    {
        // Constants
        protected const string CONTEXT_MENU_PATH = "ActionEffects/";

        // Publics
        abstract public void Invoke(ABaseComponent user, ABaseComponent target, params float[] values);
        public Type[] CachedRequiredComponents
        {
            get
            {
                if (_cachedRequiredComponenets == null)
                    _cachedRequiredComponenets = RequiredComponents;
                return _cachedRequiredComponenets;
            }
        }
        virtual public int RequiredValuesCount
        => 0;

        // Privates
        private Type[] _cachedRequiredComponenets;
        virtual protected Type[] RequiredComponents
        => new Type[0];
    }
}