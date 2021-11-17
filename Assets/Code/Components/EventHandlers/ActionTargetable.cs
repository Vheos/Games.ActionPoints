namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;

    [DisallowMultipleComponent]
    sealed public class ActionTargetable : AEventSubscriber
    {
        // Events
        public Event<ABaseComponent, ABaseComponent> OnTargeterChange
        { get; } = new Event<ABaseComponent, ABaseComponent>();

        // Publics
        public ABaseComponent Targeter
        { get; private set; }
        public void GainTargeting(ABaseComponent targeter)
        {
            ABaseComponent previousTargeter = Targeter;
            Targeter = targeter;
            if (previousTargeter != Targeter)
                OnTargeterChange?.Invoke(previousTargeter, Targeter);
        }
        public void LoseTargeting()
        => GainTargeting(null);
    }
}