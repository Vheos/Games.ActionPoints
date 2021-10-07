namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;

    public class ActionWheel : AUpdatable
    {
        // Inspector
        [Range(90, 270)] public float _MaxAngle = 180f;
        [Range(0.5f, 2f)] public float _Radius = 2 / 3f;
        public QAnimVector2 _ExpandScaleAnim = new QAnimVector2();

        // Publics
        static public ActionWheel Create(GameObject prefab, ActionUI ui)
        {
            ActionWheel newWheel = Instantiate(prefab).GetComponent<ActionWheel>();
            newWheel.name = nameof(ActionWheel);
            newWheel.BecomeChildOf(ui);
            newWheel.UI = ui;

            newWheel.Initialize();
            return newWheel;
        }
        public ActionUI UI
        { get; private set; }
        public bool IsExpanded
        { get; private set; }
        public void AlignButtons(Vector2 wheelDirection, float radius, float maxAngle)
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                float angle = maxAngle * (i - _buttons.Count.Sub(1).Div(2)) / _buttons.Count.Add(1);
                Vector2 targetLocalPosition = radius * wheelDirection.Rotate(Vector3.back, angle);
                _buttons[i].MoveTo(targetLocalPosition);
            }
        }
        public void ExpandButtons()
        {
            _ExpandScaleAnim.Start(transform.localScale, Vector3.one);
            foreach (var button in _buttons)
                button.RecieveMouseEvents = true;
            AlignButtons(GetWheelDirection(UI.Character.GetComponent<SnapTo>()), _Radius, _MaxAngle);
            IsExpanded = true;
        }
        public void CollapseButtons()
        {
            _ExpandScaleAnim.Start(transform.localScale, Vector3.zero);
            foreach (var button in _buttons)
                button.RecieveMouseEvents = false;
            IsExpanded = false;
        }
        public void Toggle()
        {
            if (IsExpanded)
                CollapseButtons();
            else
                ExpandButtons();
        }

        // Privates
        static private Vector2 GetWheelDirection(SnapTo snapTo)
        {
            if (snapTo == null || !snapTo.IsActive)
                return Vector2.up;

            Vector3 edgeFrom = snapTo.SnappableOffset.Rotate(snapTo._Snappable.transform.forward, -1);
            Vector3 edgeTo = snapTo.SnappableOffset.Rotate(snapTo._Snappable.transform.forward, +1);
            Vector2 screenDirection = edgeFrom.ScreenOffsetTo(edgeTo, CameraManager.FirstActive).XY().PerpendicularCCW().normalized;
            return screenDirection;
        }
        private List<ActionButton> _buttons;
        private void Initialize()
        {
            _buttons = new List<ActionButton>();
            foreach (var action in UI.Character._Actions)
                _buttons.Add(ActionButton.Create(UI._ButtonPrefab, this, action));
            CollapseButtons();

            if (TryGetComponent<MoveTowards>(out var moveTowards))
                moveTowards._Target = UI.Character.transform;
            if (TryGetComponent<RotateAs>(out var rotateAs))
                rotateAs._Target = CameraManager.FirstActive.transform;

        }

        // Mono
        public override void PlayUpdate()
        {
            base.PlayUpdate();
            if (_ExpandScaleAnim.IsActive)
                transform.localScale = _ExpandScaleAnim.Value;
        }
    }
}