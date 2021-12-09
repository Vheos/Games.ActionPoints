namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.General;

    abstract public class AUIComponent : AAutoSubscriber
    {
        // Privates
        protected UIBase Base
        { get; private set; }
        protected Character Character
        => Base.Character;
        private void TryFindUIBase()
        {
            if (this.TryAs<UIBase>(out var thisAsBase))
                Base = thisAsBase;
            else if (transform.parent.TryNonNull(out var parent)
            && parent.TryGetComponent<AUIComponent>(out var parentUIComponent))
                Base = parentUIComponent.Base;

            if (Base == null)
                WarningUIBaseNotFound(this);
        }
        private void WarningUIBaseNotFound(Component component)
        => Debug.LogWarning($"{GetType().Name} / UIBaseNotFound   -   gameObject {component.gameObject.name}, component {component.GetType().Name}");

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            TryFindUIBase();
            name = GetType().Name;
        }
    }
}