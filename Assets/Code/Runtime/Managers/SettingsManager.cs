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

        // Publics
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