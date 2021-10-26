namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;

    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-1)]
    public class UIManager : ABaseComponent
    {
        // Constants
        private const string UI_ROOT_NAME = "UI";

        // Inspector
        [SerializeField] protected UISettings _Settings = null;
        [SerializeField] protected UIPrefabs _Prefabs = null;

        // Publics
        static public UISettings Settings
        => _instance._Settings;
        static public UIPrefabs Prefabs
        => _instance._Prefabs;
        static public Transform HierarchyRoot
        { get; private set; }

        // Privates
        static private UIManager _instance;

        // Play
        public override void PlayAwake()
        {
            base.PlayAwake();
            _instance = this;
            HierarchyRoot = new GameObject(UI_ROOT_NAME).transform;
        }
    }
}