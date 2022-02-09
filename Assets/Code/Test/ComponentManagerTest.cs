namespace Vheos.Games.ActionPoints.Test
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.General;
    using Tools.Extensions.Reflection;

    public class ComponentManagerTest : MonoBehaviour
    {
        [field: SerializeField] public ABaseComponent Manager { get; private set; }
        public bool LogComponents;
        public bool InstantiateComponent;

        private void Update()
        {
            if (Manager == null)
                return;

            if (LogComponents.Consume())
                Manager.TryInvokeMethodVoid(nameof(LogComponents));
            if (InstantiateComponent.Consume())
                Manager.GetType().BaseType.InvokeMethodVoid(nameof(InstantiateComponent));
        }
    }
}