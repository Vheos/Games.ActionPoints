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
        public UIWheel Wheel
        { get; private set; }
        public UIActionPointsBar PointsBar
        { get; private set; }
        public UITargetingLine TargetingLine
        { get; private set; }
        public UIPopupHandler PopupHandler
        { get; private set; }

        // Play
        public override void PlayStart()
        {
            base.PlayStart();
            name = GetType().Name;
            UI = this;

            transform.position = Character.transform.position;
            Wheel = this.CreateChildComponent<UIWheel>(UIManager.Settings.Prefab.Wheel);
            PointsBar = this.CreateChildComponent<UIActionPointsBar>(UIManager.Settings.Prefab.ActionPointsBar);
            TargetingLine = this.CreateChildComponent<UITargetingLine>(UIManager.Settings.Prefab.TargetingLine);
            PopupHandler = this.CreateChildComponent<UIPopupHandler>(UIManager.Settings.Prefab.PopupHandler);
        }
    }
}

/*
public void ToggleWheel()
=> Wheel.Toggle();
public void CollapseOtherWheels()
{
    foreach (var wheel in FindObjectsOfType<UIWheel>())
        if (wheel.IsExpanded && wheel != Wheel)
            wheel.CollapseButtons();
}
public void ExpandWheel()
{
    if (!Wheel.IsExpanded)
        Wheel.ExpandButtons();
}
public void StartTargeting(Transform from, Transform to)
{
    CursorManager.SetCursorDistance(from);
    TargetingLine.Activate(from, to);
}
public void StopTargeting()
=> TargetingLine.Deactivate();
public bool TryGetCursorCharacter(out Character target)
=> TargetingLine.TryGetCursorCharacter(out target);
public void NotifyExhausted()
=> PointsBar.NotifyExhausted();
public void PopDamage(Vector3 position, float damage, int wounds)
=> PopupHandler.PopDamage(position, damage, wounds);
*/