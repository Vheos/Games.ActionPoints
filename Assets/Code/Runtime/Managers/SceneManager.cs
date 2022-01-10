namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;
    using UnityEngine.SceneManagement;
    using Tools.Extensions.General;
    using Tools.Extensions.Collections;
    using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
    using System.Collections.Generic;
    using System.Linq;
    using Vheos.Tools.Extensions.UnityObjects;

    [DisallowMultipleComponent]
    public class SceneManager : AManager<SceneManager>
    {
        // Inspector
        [ScenePicker] [SerializeField] protected string[] _Scenes;

        // Privates
        private string _currentScenePath;
        private void TryChangeTo(string scenePath)
        {
            if (IsLoaded(scenePath))
            {
                _currentScenePath = scenePath;
                return;
            }

            if (IsLoaded(_currentScenePath))
                Unload(_currentScenePath);

            LoadAdditively(scenePath);
            _currentScenePath = scenePath;
        }
        private bool IsLoaded(string scenePath)
        => LoadedScenePaths.Contains(scenePath);
        private void LoadAdditively(string scenePath)
        => UnitySceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
        private void Unload(string scenePath)
        => UnitySceneManager.UnloadSceneAsync(scenePath);
        private IEnumerable<string> LoadedScenePaths
        {
            get
            {
                for (int i = 0; i < UnitySceneManager.sceneCount; i++)
                    yield return UnitySceneManager.GetSceneAt(i).path;
            }
        }
        private void OnUpdate()
        {
            if (_Scenes.Length >= 2
            && KeyCode.Tab.Pressed()
            && _Scenes.TryFindIndex(_currentScenePath, out var currentIndex))
                TryChangeTo(_Scenes[currentIndex.Add(1).PosMod(_Scenes.Length)]);
        }

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            if (_Scenes.TryGetNonNull(0, out var startingScenePath))
                TryChangeTo(startingScenePath);
        }
        protected override void DefineAutoSubscriptions()
        {
            base.DefineAutoSubscriptions();
            SubscribeTo(Get<Updatable>().OnUpdate, OnUpdate);
        }
    }
}