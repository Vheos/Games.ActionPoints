namespace Vheos.Games.ActionPoints.Test
{
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Vheos.Tools.Extensions.UnityObjects;

    public class InstantiateTest : MonoBehaviour
    {
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField] public InputAction InputAction { get; private set; }
        [field: SerializeField] public bool CallOnAwake { get; private set; }
        [field: SerializeField] public bool DestroyRightAfter { get; private set; }

        private void TryDebugInstantiate()
        {
            if (Prefab == null)
                return;

            Debug.Log($"Calling Instantiate...");
            var newComponent = gameObject.CreateSiblingComponent(Prefab.GetComponent<MonoBehaviourTest>());
            Debug.Log($"Called!");

            if (!DestroyRightAfter)
                return;

            Debug.Log($"Calling Destroy...");
            GameObject.Destroy(newComponent.gameObject);
            Debug.Log($"Called!");
        }
        private void Awake()
        {
            InputAction.Enable();
            if (CallOnAwake)
                TryDebugInstantiate();
        }
        private void Update()
        {
            if (InputAction.triggered)
                TryDebugInstantiate();
        }
    }
}