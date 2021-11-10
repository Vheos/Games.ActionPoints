namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;

    [DisallowMultipleComponent]
    sealed public class SpriteChangable : AEventSubscriber
    {
        // Events
        public Event<Sprite, Sprite> OnSpriteChanged
        { get; } = new Event<Sprite, Sprite>();

        // Privates
        private Sprite _previousSprite;
        private void TryInvokeEvents()
        {
            Sprite currentSprite = Get<SpriteRenderer>().sprite;
            if (_previousSprite != currentSprite)
                OnSpriteChanged?.Invoke(_previousSprite, currentSprite);
            _previousSprite = currentSprite;
        }

        // Play
        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            SubscribeTo(GetHandler<Updatable>().OnUpdated, TryInvokeEvents);
        }
    }
}