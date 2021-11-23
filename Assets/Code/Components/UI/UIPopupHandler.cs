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
        public void PopDamage(Vector3 position, float damage, bool isWound)
        {
            UIDamagePopup newPopup = this.CreateChildComponent<UIDamagePopup>(UIManager.Settings.Prefab.DamagePopup);
            _isOddPopup = !_isOddPopup;
            newPopup.Initialize(position, damage, isWound, _isOddPopup);
        }

        // Privates
        private bool _isOddPopup;
    }
}