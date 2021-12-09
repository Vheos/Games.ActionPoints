namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;

    [DisallowMultipleComponent]
    sealed public class SpriteChangable : AAutoSubscriber
    {
        // Events
        public AutoEvent<Sprite, Sprite> OnChangeSprite
        { get; } = new AutoEvent<Sprite, Sprite>();

        // Privates
        private Sprite _previousSprite;
        private void TryInvokeEvents()
        {
            Sprite currentSprite = Get<SpriteRenderer>().sprite;
            if (_previousSprite != currentSprite)
                OnChangeSprite?.Invoke(_previousSprite, currentSprite);
            _previousSprite = currentSprite;
        }

        // Play
        protected override void DefineAutoSubscriptions()
        {
            base.DefineAutoSubscriptions();
            SubscribeTo(Get<Updatable>().OnUpdate, TryInvokeEvents);
        }
    }
}