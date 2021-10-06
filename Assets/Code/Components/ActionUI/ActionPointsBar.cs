namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;

    public class ActionPointsBar : AUpdatable
    {
        // Publics
        static public ActionPointsBar Create(GameObject prefab, ActionUI ui)
        {
            ActionPointsBar newPointsBar = Instantiate(prefab).GetComponent<ActionPointsBar>();
            newPointsBar.name = nameof(ActionPointsBar);
            newPointsBar.BecomeChildOf(ui);
            newPointsBar.UI = ui;

            newPointsBar.Initialize();
            return newPointsBar;
        }
        public ActionUI UI
        { get; private set; }

        // Privates
        private void Initialize()
        {
        }
    }
}