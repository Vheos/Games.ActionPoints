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
        public void AlignButtons(Vector2 wheelDirection, float radius, float maxAngle)
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                float angle = maxAngle * (i - _buttons.Count.Sub(1).Div(2)) / _buttons.Count.Add(1);
                Vector2 targetLocalPosition = radius * wheelDirection.Rotate(Vector3.back, angle);
                _buttons[i].MoveTo(targetLocalPosition);
            }
        }
        public void Show()
        {
            this.GOActivate();
            transform.AnimateLocalScale(this, Vector3.one, Settings.AnimDuration);
            foreach (var button in _buttons)
                button.GetComponent<Mousable>().enabled = true;

            AlignButtons(GetWheelDirection(), Settings.Radius, Settings.MaxAngle);
            IsExpanded = true;
        }
        public void Hide(bool instantly = false)
        {
            transform.AnimateLocalScale(this, Vector3.zero, instantly ? 0f : Settings.AnimDuration, this.GODeactivate);
            foreach (var button in _buttons)
                button.GetComponent<Mousable>().enabled = false;
            IsExpanded = false;
        }
        public void Toggle()
        {
            if (IsExpanded)
                Hide();
            else
                Show();
        }

        // Privates
        private UISettings.WheelSettings Settings
        => UIManager.Settings.Wheel;
        private Vector2 GetWheelDirection()
        {
            Vector3 midpoint;
            if (Base.Character.Team.TryNonNull(out var team)
            && team.Count > 1)
                midpoint = team.Midpoint;
            else if (Base.Character.Combat.TryNonNull(out var combat))
                midpoint = combat.Midpoint;
            else
                return Vector3.up;

            return midpoint.ScreenOffsetTo(Base.Character.transform.position, CameraManager.FirstActive).normalized;
        }
        private List<UIButton> _buttons;

        // Play
        protected override void PlayStart()
        {
            base.PlayStart();

            if (TryGetComponent<MoveTowards>(out var moveTowards))
                moveTowards.Target = Base.Character.transform;
            if (TryGetComponent<RotateAs>(out var rotateAs))
                rotateAs.Target = CameraManager.FirstActive.transform;

            _buttons = new List<UIButton>();
            foreach (var action in Base.Character.Actions)
            {
                UIButton newButton = this.CreateChildComponent<UIButton>(UIManager.Settings.Prefab.Button);
                newButton.Action = action;
                _buttons.Add(newButton);
            }

            Hide(true);
        }
    }
}