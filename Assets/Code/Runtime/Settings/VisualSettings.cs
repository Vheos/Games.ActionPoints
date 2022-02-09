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

        // Publics
        [field: SerializeField] public GeneralSettings General { get; private set; }
        #region struct
        [Serializable]
        public struct GeneralSettings
        {
            public Mesh SpriteMesh;
            public Material ShadowSpriteMaterial;
        }
        #endregion
        [field: SerializeField] public UICursorSettings UICursor { get; private set; }
        #region struct
        [Serializable]
        public struct UICursorSettings
        {
            public Sprite IdleSprite;
            public Sprite PressSprite;
            [Range(0.5f, 2f)] public float PressScale;
            [Range(0.5f, 2f)] public float PressColorScale;
            [Range(0f, 1f)] public float PressDuration;
            [Range(0f, 1f)] public float ReleaseDuration;
        }
        #endregion
        [field: SerializeField] public UITargetingLineSettings UITargetingLine { get; private set; }
        #region struct
        [Serializable]
        public struct UITargetingLineSettings
        {
            [Range(0f, 1f)] public float StartOpacity;
            [Range(0f, 1f)] public float StartWidth;
            [Range(0f, 1f)] public float EndOpacity;
            [Range(0f, 1f)] public float EndWidth;
            [Range(1f, 100f)] public float Tiling;
            [Range(0f, 1f)] public float ExpandDuration;
            [Range(0f, 1f)] public float CollapseDuration;
        }
        #endregion
        [field: SerializeField] public CommonSelectableSettings CommonSelectable { get; private set; }
        #region struct
        [Serializable]
        public struct CommonSelectableSettings
        {
            [Range(0.5f, 2f)] public float HighlightScale;
            [Range(0.5f, 2f)] public float HighlightColorScale;
            [Range(0.5f, 2f)] public float PressScale;
            [Range(0.5f, 2f)] public float PressColorScale;
            [Range(0f, 1f)] public float GainHighlightDuration;
            [Range(0f, 1f)] public float LoseHighlightDuration;
            [Range(0f, 1f)] public float PressDuration;
            [Range(0f, 1f)] public float ReleaseDuration;
        }
        #endregion
        [field: SerializeField] public SpriteOutlineSettings SpriteOutline { get; private set; }
        #region struct
        [Serializable]
        public struct SpriteOutlineSettings
        {
            public Material Material;
            [Range(0f, 1f)] public float Thickness;
            [Range(0f, 1f)] public float ExpandDuration;
            [Range(0f, 1f)] public float CollapseDuration;
        }
        #endregion
        [field: SerializeField] public ActionUIElementSettings ActionUIElement { get; private set; }
        #region struct
        [Serializable]
        public struct ActionUIElementSettings
        {
            [Range(0f, 1f)] public float CreateElementDuration;
            [Range(0f, 1f)] public float DestroyElementDuration;
            [Range(0f, 1f)] public float MoveElementDuration;
            [Range(0f, 1f)] public float ExpandGroupDuration;
            [Range(0f, 1f)] public float CollapseGroupDuration;
        }
        #endregion
        [field: SerializeField] public ActionPointSettings ActionPoint { get; private set; }
        #region struct
        [Serializable]
        public struct ActionPointSettings
        {
            public Material Material;
            public Texture2D ActionShape;
            public Texture2D FocusShape;
            [Range(0f, 1f)] public float PartialOpacity;
            [Range(0f, 1f)] public float FullOpacity;
            [Range(0f, 1f)] public float ChangeOpacityDuration;
            [Range(0f, 1f)] public float ChangeShapeDuration;
            [Range(-0.25f, +0.25f)] public float Spacing;
            [Range(0f, 1f)] public float VisualProgressHalfTime;
        }
        #endregion
        [field: SerializeField] public ActionButtonSettings ActionButton { get; private set; }
        #region struct
        [Serializable]
        public struct ActionButtonSettings
        {
            [Range(0f, 1f)] public float Radius;
            [Range(0.5f, 2f)] public float HighlightScale;
            [Range(0.5f, 2f)] public float PressScale;
            [Range(0f, 1f)] public float UnusableColorScale;
            [Range(0f, 1f)] public float ChangeUsabilityDuration;
        }
        #endregion
    }
}