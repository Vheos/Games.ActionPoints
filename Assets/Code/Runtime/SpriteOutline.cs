namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.UnityObjects;

    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(SpriteOutlineMProps))]
    [RequireComponent(typeof(SpriteChangable))]
    public class SpriteOutline : ABaseComponent
    {
        // Inspector
        [field: SerializeField] public Color Color { get; private set; } = Color.white;

        // Publics
        public void Show(bool instantly = false)
        {
            var _mprops = Get<SpriteOutlineMProps>();
            _outlineRenderer.gameObject.SetActive(true);
            _outlineRenderer.NewTween(ConflictResolution.Interrupt)
                .SetDuration(this.Settings().ExpandDuration)
                .AddPropertyModifier(v => _mprops.Thickness += v, this.Settings().Thickness - _mprops.Thickness)
                .Color(ColorComponent.SpriteRenderer, Color)
                .FinishIf(instantly);
        }
        public void Hide(bool instantly = false)
        { 
            var _mprops = Get<SpriteOutlineMProps>();
            _outlineRenderer.NewTween(ConflictResolution.Interrupt)
              .SetDuration(this.Settings().CollapseDuration)
              .AddPropertyModifier(v => _mprops.Thickness += v, 0f - _mprops.Thickness)
              .Alpha(ColorComponent.SpriteRenderer, 0f)
              .AddEventsOnFinish(() => _outlineRenderer.gameObject.SetActive(false))
              .FinishIf(instantly);
        }

        // Private
        private SpriteRenderer _outlineRenderer;

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            _outlineRenderer = this.CreateChildComponent<SpriteRenderer>(nameof(SpriteOutline));
            _outlineRenderer.sharedMaterial = this.Settings().Material;
            Get<SpriteOutlineMProps>().Initialize(_outlineRenderer);
            
            _outlineRenderer.sprite = Get<SpriteRenderer>().sprite;
            Get<SpriteChangable>().OnChangeSprite.SubEnableDisable(this, (from, to) => _outlineRenderer.sprite = to);
            Hide(true);          
        }
    }
}