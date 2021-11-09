/*
namespace Vheos.Tools.UnityCore
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;    

    abstract public class AEventHandler : AEventSubscriber
    {

        // Editor
#if UNITY_EDITOR
        [ContextMenu(nameof(PrintHandlerInfo))]
        public void PrintHandlerInfo()
        {
            Debug.Log($"{GetType().Name}");
            foreach (var @event in _events)
            {
                Debug.Log($"\t{@event.GetType().Name} ({@event.Actions.Length})");
                foreach (var action in @event.Actions)
                    Debug.Log($"\t\t{action.Target.GetType().FullName}");
            }
            Debug.Log($"");
        }
#endif
    }
}
*/