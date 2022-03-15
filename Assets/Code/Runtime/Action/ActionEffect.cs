namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using System.Collections.Generic;

    abstract public class ActionEffect : ScriptableObject
    {
        // Constants
        protected const string CONTEXT_MENU_PATH = "ActionEffects/";

        // Publics
        abstract public void Invoke(ABaseComponent @subject, ABaseComponent @object, float[] values, ActionStats actionStats);
        virtual public Type[] SubjectRequiredComponents
        => new Type[0];
        virtual public Type[] ObjectRequiredComponents
        => new Type[0];
        virtual public int RequiredValuesCount
        => 0;
    }
}