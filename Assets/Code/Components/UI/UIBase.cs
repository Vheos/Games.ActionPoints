namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.UnityObjects;

    public class UIBase : ABaseComponent, IUIHierarchy
    {
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

        // Play
        public override void PlayStart()
        {
            base.PlayStart();
            name = GetType().Name;
            UI = this;

            transform.position = Character.transform.position;
            _wheel = this.CreateChildComponent<UIWheel>(UIManager.Settings.Prefab.Wheel);
            _pointsBar = this.CreateChildComponent<UIActionPointsBar>(UIManager.Settings.Prefab.ActionPointsBar);
            _targetingLine = this.CreateChildComponent<UITargetingLine>(UIManager.Settings.Prefab.TargetingLine);
            _popupHandler = this.CreateChildComponent<UIPopupHandler>(UIManager.Settings.Prefab.PopupHandler);
        }
    }
}