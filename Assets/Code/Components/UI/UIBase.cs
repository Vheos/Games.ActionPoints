namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;

    public class UIBase : AUIComponent
    {
        // Publics
        public new Character Character
        { get; private set; }
        public UIWheel Wheel
        { get; private set; }
        public UIActionPointsBar PointsBar
        { get; private set; }
        public UITargetingLine TargetingLine
        { get; private set; }
        public UIPopupHandler PopupHandler
        { get; private set; }
        public void Initialize(Character character)
        {
            Character = character;
            Wheel = this.CreateChildComponent<UIWheel>(UIManager.Settings.Prefab.Wheel);
            Wheel.Initialize();
            PointsBar = this.CreateChildComponent<UIActionPointsBar>(UIManager.Settings.Prefab.ActionPointsBar);
            PointsBar.Initialize();
            TargetingLine = this.CreateChildComponent<UITargetingLine>(UIManager.Settings.Prefab.TargetingLine);
            TargetingLine.Initialize();
            PopupHandler = this.CreateChildComponent<UIPopupHandler>(UIManager.Settings.Prefab.PopupHandler);
            PopupHandler.Initialize();

            transform.position = Character.transform.position;
        }
    }
}