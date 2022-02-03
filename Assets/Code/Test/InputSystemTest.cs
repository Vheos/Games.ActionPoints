namespace Vheos.Games.ActionPoints.Test
{
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class InputSystemTest : MonoBehaviour
    {
        private void Awake()
        {
            foreach (var control in Mouse.current.allControls)
            {
                Debug.Log($"{control.name}");
            }
        }
        private void Update()
        {
           
        }
    }
}