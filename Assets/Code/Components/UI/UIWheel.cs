namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.General;

    public class UIWheel : APlayable, IUIHierarchy
    {
        // Inspector
        [Range(0f, 1f)] public float _AnimDuration;

        // Publics
        public UIBase UI
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
            transform.AnimateLocalScale(this, Vector3.one, _AnimDuration);
            foreach (var button in _buttons)
                button.RecieveMouseEvents = true;


            AlignButtons(GetWheelDirection(), UI._WheelRadius, UI._WheelMaxAngle);
            IsExpanded = true;
        }
        public void CollapseButtons()
        {
            transform.AnimateLocalScale(this, Vector3.zero, _AnimDuration);
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
        private Vector2 GetWheelDirection()
        {
            if (!UI.Character.Team.TryNonNull(out var team)
            || team.Count <= 1)
                return Vector3.up;

            return team.Midpoint.ScreenOffsetTo(UI.Character.transform.position, CameraManager.FirstActive).normalized;            
        }
        private List<UIButton> _buttons;

        // Mono
        public override void PlayStart()
        {
            base.PlayStart();
            name = GetType().Name;
            UI = transform.parent.GetComponent<IUIHierarchy>().UI;

            if (TryGetComponent<MoveTowards>(out var moveTowards))
                moveTowards._Target = UI.Character.transform;
            if (TryGetComponent<RotateAs>(out var rotateAs))
                rotateAs._Target = CameraManager.FirstActive.transform;

            _buttons = new List<UIButton>();
            foreach (var action in UI.Character._Actions)
            {
                UIButton newButton = this.CreateChildComponent<UIButton>(UI._PrefabButton);
                newButton.Action = action;
                _buttons.Add(newButton);
            }
            CollapseButtons();
        }
    }
}