namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    [CreateAssetMenu(fileName = nameof(VisualSettings), menuName = CONTEXT_MENU_PATH + nameof(VisualSettings))]
    public class VisualSettings : ScriptableObject
    {
        // Constants
        protected const string CONTEXT_MENU_PATH = "Settings/";

        // Inspector
        [Range(0f, 1f)]  public float TestFloat;
        public ActionButtonVisualSettings ActionButton;

        [Serializable]
        public struct ActionButtonVisualSettings
        {
            [Range(0f, 1f)] public float TestFloat;
        }
    }
}