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
            foreach (var action in Character.Get<Actionable>().Actions.Value)
            {
                UIButton newButton = this.CreateChildComponent<UIButton>(UIManager.Settings.Prefab.Button);
                newButton.Initialize(action);
                _buttons.Add(newButton);
            }

            if (TryGetComponent<MoveTowards>(out var moveTowards))
                moveTowards.Target = Character.transform;
            if (TryGetComponent<RotateAs>(out var rotateAs))
                rotateAs.Target = CameraManager.FirstActive.transform;
            Hide(true);
        }
        public void Show()
        {
            this.GOActivate();
            transform.AnimateLocalScale(this, Vector3.one, Settings.AnimDuration);
            foreach (var button in _buttons)
                button.Get<Mousable>().enabled = true;

            AlignButtons(GetWheelDirection(), Settings.Radius, Settings.MaxAngle);
            IsExpanded = true;
        }
        public void Hide(bool instantly = false)
        {
            transform.AnimateLocalScale(this, Vector3.zero, instantly ? 0f : Settings.AnimDuration, this.GODeactivate);
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
        public void AlignButtons(Vector2 wheelDirection, float radius, float maxAngle)
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                float angle = maxAngle * (i - _buttons.Count.Sub(1).Div(2)) / _buttons.Count.Add(1);
                Vector2 targetLocalPosition = radius * wheelDirection.Rotate(Vector3.back, angle);
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