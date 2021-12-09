namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;

    abstract public class AInitializableSO : ScriptableObject
    {
        // Privates
        abstract internal void TryInitialize();
    }
}