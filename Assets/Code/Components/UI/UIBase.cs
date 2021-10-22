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

        [Header("Prefabs")]
        public GameObject _PrefabTargetingLine = null;
        public GameObject _PrefabActionPointsBar = null;
        public GameObject _PrefabActionPoint = null;
        public GameObject _PrefabWound = null;
        public GameObject _PrefabWheel = null;
        public GameObject _PrefabButton = null;
        public GameObject _PrefabCostPointsBar = null;
        public GameObject _PrefabCostPoint = null;
        public GameObject _PrefabPopupHandler = null;
        public GameObject _PrefabDamagePopup = null;

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
            CursorManager.SetCursorDistance(from);
            _targetingLine.Activate(from, to);
        }
        public void StopTargeting()
        => _targetingLine.Deactivate();
        public bool TryGetCursorCharacter(out Character target)
        => _targetingLine.TryGetCursorCharacter(out target);
        public void NotifyExhausted()
        => _pointsBar.NotifyExhausted();
        public void PopDamage(Vector3 position, float damage, int wounds)
        => _popupHandler.PopDamage(position, damage, wounds);

        // Privates
        private UIWheel _wheel;
        private UIActionPointsBar _pointsBar;
        private UITargetingLine _targetingLine;
        private UIPopupHandler _popupHandler;

        // Mono
        public override void PlayStart()
        {
            base.PlayStart();
            name = GetType().Name;
            UI = this;

            transform.position = Character.transform.position;
            _wheel = this.CreateChildComponent<UIWheel>(_PrefabWheel);
            _pointsBar = this.CreateChildComponent<UIActionPointsBar>(_PrefabActionPointsBar);
            _targetingLine = this.CreateChildComponent<UITargetingLine>(_PrefabTargetingLine);
            _popupHandler = this.CreateChildComponent<UIPopupHandler>(_PrefabPopupHandler);
        }
    }
}