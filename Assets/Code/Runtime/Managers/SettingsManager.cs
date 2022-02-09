namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    [DisallowMultipleComponent]
    public class SettingsManager : AStaticComponent<SettingsManager>
    {
        // Inspector
        [field: SerializeField] public VisualSettings VisualSettings { get; private set; }
        [field: SerializeField] public PrefabSettings PrefabSettings { get; private set; }

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            Tween.DefaultCurve = Qurve.ValuesByProgress;
        }
    }

    static public class Settings
    {
        // Publics
        static public VisualSettings Visual
        => SettingsManager.Instance.VisualSettings;
        static public PrefabSettings Prefabs
        => SettingsManager.Instance.PrefabSettings;
    }
}