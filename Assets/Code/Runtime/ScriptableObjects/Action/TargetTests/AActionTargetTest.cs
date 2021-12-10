namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;

    abstract public class AActionTargetTest : ScriptableObject
    {
        // Constants
        protected const string CONTEXT_MENU_PATH = "Action/TargetTests/";

        // Publics
        abstract public bool Invoke(Targeter user, Targetable target);

        // Defines
        [System.Serializable]
        public class Data
        {
            // Inspector     
            public AActionTargetTest TargetTest = null;
            public bool TestForTrue = true;

            // Publics
            public bool Test(Targeter user, Targetable target)
            => TestForTrue == TargetTest.Invoke(user, target);
        }
    }
}