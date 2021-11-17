namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;

    [DisallowMultipleComponent]
    sealed public class UIManager : AManager<UIManager>
    {
        // Constants
        private const string UI_ROOT_NAME = "UI";

        // Inspector
        [SerializeField] private UISettings _Settings = null;

        // Publics
        static public UISettings Settings
        => _instance._Settings;
        static public Transform HierarchyRoot
        { get; private set; }

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            HierarchyRoot = new GameObject(UI_ROOT_NAME).transform;
        }
    }
}