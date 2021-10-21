namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;

    public class UIPopupHandler : AUpdatable, IUIHierarchy
    {
        // Inspector

        // Publics
        public UIBase UI
        { get; private set; }
        public void PopDamage(Vector3 position, float damage, int wounds)
        {
            UIDamagePopup newPopup = this.CreateChildComponent<UIDamagePopup>(UI._PrefabDamagePopup);
            newPopup.Initialize(position, damage, wounds);          
        }

        // Mono
        public override void PlayStart()
        {
            base.PlayStart();
            name = GetType().Name;
            UI = transform.parent.GetComponent<IUIHierarchy>().UI;
        }
    }
}