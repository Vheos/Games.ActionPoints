namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.General;

    abstract public class AUIComponent : AEventSubscriber
    {
        // Privates
        protected readonly GUID _animGUID = GUID.New;
        protected OptionalParameters OptionalsInterrupt;
        protected OptionalParameters OptionalsInterruptAndDeactivateSelf;
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
            OptionalsInterrupt = new OptionalParameters() { ConflictResolution = ConflictResolution.Interrupt, GUID = _animGUID };
            OptionalsInterruptAndDeactivateSelf = new OptionalParameters() { ConflictResolution = ConflictResolution.Interrupt, GUID = _animGUID, EventInfo = new EventInfo(this.GODeactivate) };

            TryFindUIBase();
            name = GetType().Name;
        }
    }
}