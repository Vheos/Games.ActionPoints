namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.General;

    public class UIWheel : AUIComponent
    {
        // Publics
        public bool IsExpanded
        { get; private set; }
        public void Initialize()
        {
            foreach (var action in Character.Get<Actionable>().Actions)
            {
                UIButton newButton = this.CreateChildComponent<UIButton>(UIManager.Settings.Prefab.Button);
                newButton.Initialize(action);
                _buttons.Add(newButton);
            }

            if (TryGet<MoveTowards>(out var moveTowards))
                moveTowards.SetTarget(Character);
            if (TryGet<RotateAs>(out var rotateAs))
                rotateAs.SetTarget(CameraManager.FirstActive);
            Hide(true);
        }
        public void Show()
        {
            this.GOActivate();
            transform.AnimateLocalScale(Vector3.one, Settings.AnimDuration, _animGUID, ConflictResolution.Interrupt);
            foreach (var button in _buttons)
                button.Get<Mousable>().enabled = true;

            AlignButtons();
            IsExpanded = true;
        }
        public void Hide(bool instantly = false)
        {
            transform.AnimateLocalScale(Vector3.zero, instantly ? 0f : Settings.AnimDuration, new EventInfo(this.GODeactivate).InArray(), _animGUID, ConflictResolution.Interrupt);
            foreach (var button in _buttons)
                button.Get<Mousable>().enabled = false;
            IsExpanded = false;
        }
        public void Toggle()
        {
            if (IsExpanded)
                Hide();
            else
                Show();
        }
        public void AlignButtons()
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                float angle = Settings.MaxAngle * (i - _buttons.Count.Sub(1).Div(2)) / _buttons.Count.Add(1);
                Vector2 targetLocalPosition = Settings.Radius * GetWheelDirection().Rotate(Vector3.back, angle);
                _buttons[i].MoveTo(targetLocalPosition);
            }
        }

        // Privates
        private readonly List<UIButton> _buttons = new List<UIButton>();
        private UISettings.WheelSettings Settings
        => UIManager.Settings.Wheel;
        private Vector2 GetWheelDirection()
        {
            Vector3 midpoint;
            if (Character.Team.TryNonNull(out var team)
            && team.Count > 1)
                midpoint = team.Midpoint;
            else if (Character.Combat.TryNonNull(out var combat))
                midpoint = combat.Midpoint;
            else
                return Vector3.up;

            return midpoint.ScreenOffsetTo(Character.transform.position, CameraManager.FirstActive).normalized;
        }
    }
}