namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Linq;
    using UnityEngine;
    using Tools.UnityCore;
    using static UIManager;

    [DisallowMultipleComponent]
    public class UIManager : AManager<UIManager>
    {
        // Constants
        private const string UI_ROOT_NAME = "UI";

        // Inspector
        [SerializeField] protected UISettings _Settings = null;
        [SerializeField] protected KeyCode[] _PrimaryButtons = new KeyCode[1];
        [SerializeField] protected KeyCode[] _SecondaryButtons = new KeyCode[1];

        // Publics
        static public UISettings Settings
        => _instance._Settings;
        static public Transform HierarchyRoot
        { get; private set; }
        static public ButtonFunction KeyCodeToFunction(KeyCode button)
        => _instance._PrimaryButtons.Contains(button) ? ButtonFunction.Primary
        : _instance._SecondaryButtons.Contains(button) ? ButtonFunction.Secondary
        : ButtonFunction.None;

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            HierarchyRoot = new GameObject(UI_ROOT_NAME).transform;
        }

        // Defines
        public enum ButtonFunction
        {
            None = 0,
            Primary,
            Secondary,
        }
    }

    static public class UIManager_Extensions
    {
        static public ButtonFunction ToFunction(this KeyCode keyCode)
        => KeyCodeToFunction(keyCode);
    }
}