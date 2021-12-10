namespace Vheos.Games.ActionPoints.Test
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.General;
    using System.Collections.Generic;

    [CreateAssetMenu(fileName = nameof(ScriptableObjectTest), menuName = nameof(ScriptableObjectTest))]
    public class ScriptableObjectTest : ScriptableObject
    {
        void Awake()
        => Debug.Log($"{Time.frameCount} - Awake");
        void OnEnable()
        => Debug.Log($"{Time.frameCount} - OnEnable");
        void Start()
        => Debug.Log($"{Time.frameCount} - Start");
        void Update()
        => Debug.Log($"{Time.frameCount} - Update");
        void LateUpdate()
        => Debug.Log($"{Time.frameCount} - LateUpdate");
        void FixedUpdate()
        => Debug.Log($"{Time.frameCount} - FixedUpdate");
        void OnDisable()
        => Debug.Log($"{Time.frameCount} - OnDisable");
        void OnDestroy()
        => Debug.Log($"{Time.frameCount} - OnDestroy");
    }
}