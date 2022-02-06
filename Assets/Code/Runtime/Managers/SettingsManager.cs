namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    [DisallowMultipleComponent]
    public class SettingsManager : AStaticComponent<SettingsManager>
    {
        // Inspector
       [SerializeField] protected VisualSettings _VisualSettings;
        [SerializeField] protected PrefabSettings _PrefabSettings;        

        // Publics
        static public VisualSettings VisualSettings
        => _instance._VisualSettings;
        static public PrefabSettings PrefabSettings
        => _instance._PrefabSettings;

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
        => SettingsManager.VisualSettings;
        static public PrefabSettings Prefabs
        => SettingsManager.PrefabSettings;  
    }
}