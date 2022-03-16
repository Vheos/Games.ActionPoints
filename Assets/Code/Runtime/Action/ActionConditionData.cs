namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    // Defines
    [Serializable]
    public struct ActionConditionData
    {
        // Inspector    
        public ActionCondition Condition;
        public bool Invert;
        public float[] Values;

        // Publics
        public bool Check(Actionable user, Targetable target)
        => Condition.Check(user, target, Values) != Invert;
    }
}