namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.Math;

    [CreateAssetMenu(fileName = nameof(UISettings), menuName = nameof(UISettings))]
    public class UISettings : ScriptableObject
    {
        // Inspector
        [SerializeField] protected PrefabSettings _Prefab;
        [SerializeField] protected WheelSettings _Wheel;
        [SerializeField] protected ButtonSettings _Button;
        [SerializeField] protected ActionPointSettings _ActionPoint;
        [SerializeField] protected WoundSettings _Wound;
        [SerializeField] protected TargetingLineSettings _TargetingLine;
        [SerializeField] protected DamagePopupSettings _DamagePopup;

        // Publics
        public PrefabSettings Prefab
        => _Prefab;
        public WheelSettings Wheel
        => _Wheel;
        public ButtonSettings Button
        => _Button;
        public ActionPointSettings ActionPoint
        => _ActionPoint;
        public WoundSettings Wound
        => _Wound;
        public TargetingLineSettings TargetingLine
        => _TargetingLine;
        public DamagePopupSettings DamagePopup
        => _DamagePopup;

        // Defines
        [System.Serializable]
        sealed public class PrefabSettings
        {
            public GameObject Base = null;
            public GameObject TargetingLine = null;
            public GameObject ActionPointsBar = null;
            public GameObject ActionPoint = null;
            public GameObject Wound = null;
            public GameObject Wheel = null;
            public GameObject Button = null;
            public GameObject CostPointsBar = null;
            public GameObject CostPoint = null;
            public GameObject PopupHandler = null;
            public GameObject DamagePopup = null;
        }
        [System.Serializable]
        sealed public class WheelSettings
        {
            [Range(90, 270)] public float MaxAngle = 150f;
            [Range(0.5f, 2f)] public float Radius = 1f;
            [Range(0f, 1f)] public float AnimDuration = 0.5f;
        }
        [System.Serializable]
        sealed public class ButtonSettings
        {
            [Range(0f, 1f)] public float HighlightDuration = 0.5f;
            [Range(1f, 2f)] public float HighlightScale = 1.25f;
            [Range(0f, 1f)] public float ClickDuration = 0.1f;
            [Range(0.5f, 1f)] public float ClickScale = 0.9f;
            [Range(0.5f, 1f)] public float ClickColorScale = 0.9f;
            [Range(0f, 1f)] public float UnusableDuration = 0.5f;
            public Color UnusableColor = 3.Inv().ToVector4();
            [Range(0f, 1f)] public float AnimDuration = 0.5f;
        }
        [System.Serializable]
        sealed public class ActionPointSettings
        {
            public Texture ActionShape = null;
            public Texture FocusShape = null;
            public Color BackgroundColor = Color.black;
            public Color ActionColor = Color.white;
            public Color ExhaustColor = Color.red;
            public Color FocusColor = Color.cyan;
            [Range(0f, 1f)] public float PartialProgressOpacity = 0.25f;
            [Range(0f, 1f)] public float VisualProgressHalfTime = 0.1f;
            [Range(-0.5f, +0.5f)] public float Spacing = 0f;
            [Range(1f, 3f)] public float CantUseScale = 2f;
            [Range(0f, 1f)] public float CantUseAnimDuration = 0.5f;
            [Range(0f, 1f)] public float AnimDuration = 0.5f;
            [Range(0f, 1f)] public float CostOpacity = 0.5f;
        }
        [System.Serializable]
        sealed public class WoundSettings
        {
            [Range(0f, 1f)] public float WoundAnimDuration = 0.5f;
            public Vector2 WoundAngleRandomRange = new Vector2(+30f, +60f);
            [Range(0f, 2f)] public float FadeDistance = 1f;
        }
        [System.Serializable]
        sealed public class TargetingLineSettings
        {
            [Range(0f, 10f)] public float Tiling = 5f;
            [Range(0f, 1f)] public float StartOpacity = 0.25f;
            [Range(0f, 1f)] public float StartWidth = 0.1f;
            [Range(0f, 1f)] public float EndWidthRatio = 0.25f;
            [Range(0f, 1f)] public float WidthAnimDuration = 0.5f;
        }
        [System.Serializable]
        sealed public class DamagePopupSettings
        {
            public Vector2 AngleRandomRange = new Vector2(-15, +15);
            public bool AlignTextRotationToDirection = false;
            [Range(0f, 1f)] public float Distance = 0.5f;
            [Range(0f, 1f)] public float FadeInDuration = 0.5f;
            [Range(0f, 1f)] public float StayUpDuration = 0.5f;
            [Range(0f, 1f)] public float FadeOutDuration = 0.5f;
            public AnimationCurve SizeCurve;
            public Gradient ColorCurve;
            [Range(1f, 3f)] public float WoundPulseScale = 2f;
            [Range(0f, 1f)] public float WoundPulseDuration = 0.5f;
            [Range(0f, 1f)] public float PercentSignSize = 0.5f;
        }
    }
}