namespace Vheos.Tools.UnityCore
{
    using System;
    using UnityEngine;

    public class MonoBehaviourTest : MonoBehaviour
    {
        private void Awake()
        => Debug.Log($"Awake ({gameObject.scene.name})");
        private void OnEnable()
        => Debug.Log($"OnEnable ({gameObject.scene.name})");
        private void Start()
         => Debug.Log($"Start ({gameObject.scene.name})");
        /*
        private void Update()
        => Debug.Log($"Update");
        private void LateUpdate()
        => Debug.Log($"LateUpdate");
        private void FixedUpdate()
        => Debug.Log($"FixedUpdate");
        */
        private void OnDisable()
        => Debug.Log($"OnDisable ({gameObject.scene.name})");
        private void OnDestroy()
        => Debug.Log($"OnDestroy ({gameObject.scene.name})");
    }
}