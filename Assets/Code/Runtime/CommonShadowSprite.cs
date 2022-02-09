namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;

    [RequireComponent(typeof(SpriteRenderer))]
    public class CommonShadowSprite : ABaseComponent
    {
        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            Get<SpriteRenderer>().sharedMaterial = SettingsManager.Visual.General.ShadowSpriteMaterial;            
            Get<SpriteRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
            Get<SpriteRenderer>().receiveShadows = true;
        }
    }
}