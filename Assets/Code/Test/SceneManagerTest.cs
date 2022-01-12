namespace Vheos.Games.ActionPoints.Test
{
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Tools.UnityCore;
    using Tools.Extensions.Math;

    public class SceneManagerTest : ABaseComponent
    {
        [ScenePicker] [SerializeField] protected string[] _Scenes;
        [SerializeField] protected InputAction _InputAction;

        private void Awake()
        {
            _InputAction.Enable();
        }
        private void Update()
        {
            if(_InputAction.triggered )
            {
                int currentIndex = Array.IndexOf(_Scenes, SceneManager.ActiveScene.path);
                int nextIndex = currentIndex.Add(1).PosMod(_Scenes.Length);
                SceneManager.TransitionTo(_Scenes[nextIndex]);
            }
        }
    }

}