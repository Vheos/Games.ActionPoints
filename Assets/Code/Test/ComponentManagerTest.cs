namespace Vheos.Tools.UnityCore
{
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Vheos.Tools.Extensions.General;
    using Tools.Extensions.Reflection;

    public class ComponentManagerTest : MonoBehaviour
    {
        [SerializeField] protected AAutoSubscriber _Manager;
        [SerializeField] protected string _LogMethodName = "LogComponents";
        [SerializeField] protected bool _Log;

        private void Update()
        {
            if (_Log.Consume()
            && _Manager != null)
                _Manager.TryInvokeMethodVoid(_LogMethodName);
        }
    }
}