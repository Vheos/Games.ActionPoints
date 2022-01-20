namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    abstract public class ActionEffect : ScriptableObject
    {
        // Constants
        protected const string CONTEXT_MENU_PATH = "ActionEffects/";

        // Public
        abstract public void Invoke(ABaseComponent user, ABaseComponent target, params float[] values);
        abstract public int RequiredValuesCount
        { get; }
    }
}