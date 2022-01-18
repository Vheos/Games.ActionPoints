namespace Vheos.Tools.UnityCore
{
    using System;
    using UnityEngine;
    using Tools.Extensions.General;
    using Tools.Extensions.Reflection;

    public class ComponentManagerTest : MonoBehaviour
    {
        [SerializeField] protected AAutoSubscriber _Manager;
        public bool LogComponents;
        public bool InstantiateComponent;

        private void Update()
        {
            if (_Manager == null)
                return;

            if (LogComponents.Consume())
                _Manager.TryInvokeMethodVoid(nameof(LogComponents));
            if (InstantiateComponent.Consume())
                _Manager.GetType().BaseType.InvokeMethodVoid(nameof(InstantiateComponent));
        }
    }
}