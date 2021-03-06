namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    [DisallowMultipleComponent]
    public class SettingsManager : AStaticComponent<SettingsManager>
    {
        // Constants
        public const string ASSET_MENU_PATH = "Settings/";

        // Inspector
        [field: SerializeField] public GameplaySettings GameplaySettings { get; private set; }
        [field: SerializeField] public VisualSettings VisualSettings { get; private set; }
        [field: SerializeField] public PrefabSettings PrefabSettings { get; private set; }

        // Publics
        static public GameplaySettings Gameplay
        => Instance.GameplaySettings;
        static public VisualSettings Visual
        => Instance.VisualSettings;
        static public PrefabSettings Prefabs
        => Instance.PrefabSettings;

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            Tween.DefaultCurve = Qurve.ValuesByProgress;
        }
    }
}