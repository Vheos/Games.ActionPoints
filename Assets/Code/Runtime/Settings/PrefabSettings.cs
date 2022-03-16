namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    [CreateAssetMenu(fileName = nameof(PrefabSettings), menuName = SettingsManager.ASSET_MENU_PATH + nameof(PrefabSettings))]
    public class PrefabSettings : ScriptableObject
    {
        // Inspector
        public ActionUI ActionUI;
        public ActionPointsBar ActionPointsBar;
        public ActionPoint ActionPoint;
        public ActionButtonsWheel ActionButtonsWheel;
        public ActionButton ActionButton;
        public Popup Popup;
    }
}