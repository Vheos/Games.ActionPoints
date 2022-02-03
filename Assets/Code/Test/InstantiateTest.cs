namespace Vheos.Games.ActionPoints.Test
{
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Vheos.Tools.Extensions.UnityObjects;

    public class InstantiateTest : MonoBehaviour
    {
        [SerializeField] protected GameObject _Prefab;
        [SerializeField] protected InputAction _InputAction;
        [SerializeField] protected bool _CallOnAwake;
        [SerializeField] protected bool _DestroyRightAfter;

        private void TryDebugInstantiate()
        {
            if (_Prefab == null)
                return;

            Debug.Log($"Calling Instantiate...");
            var newComponent = gameObject.CreateSiblingComponent(_Prefab.GetComponent<MonoBehaviourTest>());
            Debug.Log($"Called!");

            if (!_DestroyRightAfter)
                return;

            Debug.Log($"Calling Destroy...");
            GameObject.Destroy(newComponent.gameObject);
            Debug.Log($"Called!");
        }
        private void Awake()
        {
            _InputAction.Enable();
            if (_CallOnAwake)
                TryDebugInstantiate();
        }
        private void Update()
        {
            if (_InputAction.triggered)
                TryDebugInstantiate();
        }
    }
}