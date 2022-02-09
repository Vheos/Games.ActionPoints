namespace Vheos.Games.ActionPoints.Test
{
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Games.Core;
    using Tools.Extensions.Math;

    public class SceneManagerTest : MonoBehaviour
    {
        [field: SerializeField, ScenePicker] public string[] Scenes { get; private set; }
        [field: SerializeField] public InputAction InputAction { get; private set; }

        private void Awake()
        {
            InputAction.Enable();
        }
        private void Update()
        {
            if (InputAction.triggered)
            {
                int currentIndex = Array.IndexOf(Scenes, SceneManager.ActiveScene.path);
                int nextIndex = currentIndex.Add(1).PosMod(Scenes.Length);
                SceneManager.TransitionTo(Scenes[nextIndex]);
            }
        }
    }

}