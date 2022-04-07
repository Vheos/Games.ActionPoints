namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    [CreateAssetMenu(fileName = nameof(VisualSettings), menuName = SettingsManager.ASSET_MENU_PATH + nameof(VisualSettings))]
    public class VisualSettings : ScriptableObject
    {
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
        [field: SerializeField] public PopupSettings Popup { get; private set; }
        #region struct
        [Serializable]
        public struct PopupSettings
        {
            public Vector2 AngleRandomRange;
            public bool AlignTextRotationToDirection;
            [Range(0f, 1f)] public float Distance;
            [Range(0f, 1f)] public float FadeInDuration;
            [Range(0f, 1f)] public float StayUpDuration;
            [Range(0f, 1f)] public float FadeOutDuration;
            [Range(0f, 1f)] public float PercentSignSize;
        }
        #endregion
        [field: SerializeField] public SpecificPopupSettings DamagePopup { get; private set; }
        [field: SerializeField] public SpecificPopupSettings HealingPopup { get; private set; }
        #region struct
        [Serializable]
        public struct SpecificPopupSettings
        {
            public AnimationCurve SizeCurve;
            public Gradient ColorCurve;
            [Range(1f, 3f)] public float WoundPulseScale;
            [Range(1f, 5f)] public float WoundPulseRate;

        }
        #endregion
    }
}