namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.UnityObjects;

    public class UIPopupHandler : ABaseComponent, IUIHierarchy
    {
        // Publics
        public UIBase UI
        { get; private set; }
        public void PopDamage(Vector3 position, float damage, int wounds)
        {
            UIDamagePopup newPopup = this.CreateChildComponent<UIDamagePopup>(UIManager.Settings.Prefab.DamagePopup);
            newPopup.Initialize(position, damage, wounds);
        }

        // Play
        public override void PlayStart()
        {
            base.PlayStart();
            name = GetType().Name;
            UI = transform.parent.GetComponent<IUIHierarchy>().UI;
        }
    }
}