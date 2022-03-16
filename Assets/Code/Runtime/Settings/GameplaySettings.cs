namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    [CreateAssetMenu(fileName = nameof(GameplaySettings), menuName = SettingsManager.ASSET_MENU_PATH + nameof(GameplaySettings))]
    public class GameplaySettings : ScriptableObject
    {
        // Inspector
        public bool ResetActionPointsWhenStartingCombat;
    }
}