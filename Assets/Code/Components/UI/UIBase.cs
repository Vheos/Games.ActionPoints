namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;

    public class UIBase : AUpdatable, IUIHierarchy
    {
        // Inspector
        [Header("Buttons")]
        [Range(90, 270)] public float _WheelMaxAngle = 180f;
        [Range(0.5f, 2f)] public float _WheelRadius = 2 / 3f;
        [Range(1f, 2f)] public float _ButtonHighlightedScale = 1.25f;
        public Color _ButtonUnusableColor = 3.Inv().ToVector4();

        [Header("Points")]
        public Color _PointBackgroundColor = Color.black;
        public Color _PointActionColor = Color.white;
        public Color _PointExhaustColor = Color.red;
        public Color _PointFocusColor = Color.cyan;
        [Range(0f, 1f)] public float _PointPartialProgressOpacity = 0.25f;
        [Range(0f, 1f)] public float _PointVisualProgressHalfTime = 0.1f;
        [Range(-0.5f, +0.5f)] public float _PointsSpacing = 0;

        [Header("Targeting Line")]
        [Range(0f, 10f)] public float _LineTiling = 5f;
        [Range(0f, 1f)] public float _LineStartOpacity = 0.25f;
        [Range(0f, 1f)] public float _LineStartWidth = 0.1f;
        [Range(0f, 1f)] public float _LineEndWidthRatio = 0.25f;

        [Header("Prefabs")]
        public GameObject _PrefabTargetingLine = null;
        public GameObject _PrefabActionPointsBar = null;
        public GameObject _PrefabActionPoint = null;
        public GameObject _PrefabWheel = null;
        public GameObject _PrefabButton = null;
        public GameObject _PrefabCostPointsBar = null;
        public GameObject _PrefabCostPoint = null;

        // Publics
        public UIBase UI
        { get; private set; }
        public Character Character
        { get; set; }
        public void ToggleWheel()
        => _wheel.Toggle();
        public void CollapseOtherWheels()
        {
            foreach (var wheel in FindObjectsOfType<UIWheel>())
                if (wheel.IsExpanded && wheel != _wheel)
                    wheel.CollapseButtons();
        }
        public void ExpandWheel()
        {
            if (!_wheel.IsExpanded)
                _wheel.ExpandButtons();
        }
        public void StartTargeting(Transform from, Transform to)
        {
            CursorManager.CursorPlane = CameraManager.FirstActive.ScreenPlane(from.position);
            _targetingLine.Activate(from, to);
        }
        public void StopTargeting()
        => _targetingLine.Deactivate();
        public bool TryGetTarget(out Character target)
        => _targetingLine.TryGetTarget(out target);

        // Privates
        private UIWheel _wheel;
        private UIActionPointsBar _pointsBar;
        private UITargetingLine _targetingLine;

        // Mono
        public override void PlayStart()
        {
            base.PlayStart();
            name = GetType().Name;
            UI = this;

            _wheel = this.CreateChild<UIWheel>(_PrefabWheel);
            _pointsBar = this.CreateChild<UIActionPointsBar>(_PrefabActionPointsBar);
            _targetingLine = this.CreateChild<UITargetingLine>(_PrefabTargetingLine);
        }
    }
}