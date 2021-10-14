namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Vheos.Tools.UnityCore;

    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-1)]
    public class UIManager : AUpdatable
    {
        // Constants
        private const string UI_ROOT_NAME = "UI";

        // Inspector
        public GameObject _PrefabUIBase = null;

        // Publics
        static public GameObject PrefabUIBase
        => _instance._PrefabUIBase;
        static public Transform HierarchyRoot
        { get; private set; }
        static public Camera Camera
        { get; private set; }

        // Privates
        static private UIManager _instance;

        // Mono
        public override void PlayAwake()
        {
            base.PlayAwake();
            _instance = this;
            HierarchyRoot = new GameObject(UI_ROOT_NAME).transform;
        }
    }
}