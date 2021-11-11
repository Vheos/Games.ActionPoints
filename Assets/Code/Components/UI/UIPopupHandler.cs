namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;

    public class UIPopupHandler : AUIComponent
    {
        // Publics
        public void Initialize()
        { }
        public void PopDamage(Vector3 position, float damage, int wounds)
        {
            UIDamagePopup newPopup = this.CreateChildComponent<UIDamagePopup>(UIManager.Settings.Prefab.DamagePopup);
            newPopup.Initialize(position, damage, wounds);
        }
    }
}