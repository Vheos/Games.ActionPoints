namespace Vheos.Games.ActionPoints
{
    using Tools.UnityCore;

    [UnityEngine.DisallowMultipleComponent]
    sealed public class Updatable : APlayable
    {
        // Events
        public Event OnUpdated
        { get; } = new Event();
        public Event OnUpdatedLate
        { get; } = new Event();
        public Event OnUpdatedFixed
        { get; } = new Event();

        // Play
        private void Update()
        {
            if (isActiveAndEnabled)
                OnUpdated?.Invoke();
        }
        private void LateUpdate()
        {
            if (isActiveAndEnabled)
                OnUpdatedLate?.Invoke();
        }
        private void FixedUpdate()
        {
            if (isActiveAndEnabled)
                OnUpdatedFixed?.Invoke();
        }
    }
}