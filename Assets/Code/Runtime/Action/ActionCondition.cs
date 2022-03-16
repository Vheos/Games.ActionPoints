namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    abstract public class ActionCondition : ScriptableObject
    {
        // Constants
        public const string ASSET_MENU_PATH = "ActionConditions/";

        // Publics
        abstract public bool Check(ABaseComponent @subject, ABaseComponent @object, float[] values);
        virtual public int RequiredValuesCount
        => 0;
    }
}