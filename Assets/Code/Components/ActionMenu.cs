namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    public class ActionMenu : AUpdatable
    {
        // Publics
        public bool IsExpanded
        { get; private set; }
        public void ExpandButtons()
        {
            foreach (var button in _buttons)
                button.ExpandTo(Random.insideUnitCircle.normalized * 1f);
            IsExpanded = true;
        }
        public void CollapseButtons()
        {
            foreach (var button in _buttons)
                button.Collapse();
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
        private ActionButton[] _buttons;

        // Mono
        public override void PlayAwake()
        {
            base.PlayAwake();
            _buttons = GetComponentsInChildren<ActionButton>();
            CollapseButtons();
        }
    }
}