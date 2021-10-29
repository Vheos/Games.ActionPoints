namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.General;
    using Tools.Extensions.UnityObjects;
    using System;

    [DisallowMultipleComponent]
    sealed public class SpriteChangable : ABaseComponent
    {
        // CacheComponent
        protected override Type[] ComponentsTypesToCache => new[]
        {
            typeof(SpriteRenderer),
        };

        // Events
        public event System.Action<Sprite, Sprite> OnSpriteChange;

        // Privates
        private Sprite _previousSprite;

        // Play
        protected override void SubscribeToPlayEvents()
        {
            base.SubscribeToPlayEvents();
            Updatable.OnPlayUpdate += () =>
            {
                Sprite currentSprite = Get<SpriteRenderer>().sprite;
                if (_previousSprite != currentSprite)
                    OnSpriteChange?.Invoke(_previousSprite, currentSprite);
                _previousSprite = currentSprite;
            };
        }
    }
}