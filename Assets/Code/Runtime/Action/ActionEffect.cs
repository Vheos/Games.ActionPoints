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
        abstract public void Invoke(ABaseComponent target, float[] values, ref ActionStats actionStats);
        virtual public Type[] RequiredComponentTypes
        => new Type[0];
        virtual public int RequiredValuesCount
        => 0;
    }
}