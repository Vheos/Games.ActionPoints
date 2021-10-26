namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.Math;

    [CreateAssetMenu(fileName = nameof(UISettings), menuName = nameof(UISettings))]
    public class UISettings : ScriptableObject
    {
        // Inspector
        [Header("Buttons")]
        [Range(90, 270)] public float _WheelMaxAngle = 180f;
        [Range(0.5f, 2f)] public float _WheelRadius = 2 / 3f;

        [Header("Points")]
        public Texture _PointActionShape = null;
        public Texture _PointFocusShape = null;
        public Color _PointBackgroundColor = Color.black;
        public Color _PointActionColor = Color.white;
        public Color _PointExhaustColor = Color.red;
        public Color _PointFocusColor = Color.cyan;
        [Range(0f, 1f)] public float _PointPartialProgressOpacity = 0.25f;
        [Range(0f, 1f)] public float _PointVisualProgressHalfTime = 0.1f;
        [Range(-0.5f, +0.5f)] public float _PointsSpacing = 0;
        public float _PointCantUseScale = 2.0f;
        [Range(0f, 1f)] public float _PointCantUseAnimDuration;
        [Range(0f, 1f)] public float _PointAnimDuration;

        // UIButton
        [Range(0f, 1f)] public float _HighlightDuration = 0.5f;
        [Range(1f, 2f)] public float _HighlightScale = 1.25f;
        [Range(0f, 1f)] public float _ClickDuration = 0.1f;
        [Range(0.5f, 1f)] public float _ClickScale = 0.9f;
        [Range(0.5f, 1f)] public float _ClickColorScale = 0.9f;
        [Range(0f, 1f)] public float _UnusableDuration = 0.5f;
        public Color _UnusableColor = 3.Inv().ToVector4();
        [Range(0f, 1f)] public float _ButtonAnimDuration = 0.5f;

        // UICostPoint
        [Range(0f, 1f)] public float _Opacity = 0.5f;

        // DamagePopup
        public Vector2 _AngleRandomRange = new Vector2(-15, +15);
        public bool _AlignTextRotationToDirection = false;
        [Range(0f, 1f)] public float _Distance = 0.5f;
        [Range(0f, 1f)] public float _FadeInDuration = 0.5f;
        [Range(0f, 1f)] public float _StayUpDuration = 0.5f;
        [Range(0f, 1f)] public float _FadeOutDuration = 0.5f;
        public AnimationCurve _SizeCurve;
        public Gradient _ColorCurve;
        [Range(1f, 3f)] public float _WoundPulseScale = 2f;
        [Range(0f, 1f)] public float _WoundPulseDuration = 0.5f;
        [Range(0f, 1f)] public float _PercentSignSize = 0.5f;

        // UITargetingLine
        [Range(0f, 10f)] public float _Tiling = 5f;
        [Range(0f, 1f)] public float _StartOpacity = 0.25f;
        [Range(0f, 1f)] public float _StartWidth = 0.1f;
        [Range(0f, 1f)] public float _EndWidthRatio = 0.25f;
        [Range(0f, 1f)] public float _WidthAnimDuration = 0.5f;

        // Wheel
        [Range(0f, 1f)] public float _WheelAnimDuration;

        // Wound
        [Range(0f, 1f)] public float _WoundAnimDuration = 0.5f;
        public Vector2 _WoundAngleRandomRange = new Vector2(+30f, +60f);
        [Range(0f, 2f)] public float _FadeDistance = 1f;
    }
}