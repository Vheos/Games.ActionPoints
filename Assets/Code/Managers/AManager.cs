namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;

    [DefaultExecutionOrder(-1)]
    abstract public class AManager<T> : AEventSubscriber where T : AManager<T>
    {
        // Privates
        static protected T _instance;

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            _instance = this as T;
        }
    }
}