namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;

    public class InitializableSOManager : AManager<InitializableSOManager>
    {
        // Privates
        static private HashSet<AInitializableSO> _initializableSOs;
        static public void TryInitialize(AInitializableSO initializableSO)
        {
            if (!_initializableSOs.Contains(initializableSO))
                initializableSO.TryInitialize();
        }

        // Initializers
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static private void StaticInitialize()
        => _initializableSOs = new HashSet<AInitializableSO>();
    }
}