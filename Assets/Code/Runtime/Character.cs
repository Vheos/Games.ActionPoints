namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Vheos.Tools.Extensions.UnityObjects;
    using UnityEngine.InputSystem;
    using Vheos.Tools.Extensions.Math;

    public class Character : ABaseComponent
    {
        private void UICursorable_OnGainHighlight(AUICursor cursor, bool isFirst)
        {
            Debug.Log($"{cursor.name} -> {name}:\tOnGainHighlight, {isFirst}");
            if (isFirst)
                this.NewTween()
                    .SetDuration(0.2f)
                    .LocalScaleRatio(1.2f);
        }
        private void UICursorable_OnLoseHighlight(AUICursor cursor, bool isLast)
        {
            Debug.Log($"{cursor.name} -> {name}:\tOnLoseHighlight, {isLast}");
            if (isLast)
                this.NewTween()
                    .SetDuration(0.2f)
                    .LocalScaleRatio(1.2f.Inv());
        }
        private void UICursorable_OnPress(AUICursor cursor)
        {
            Debug.Log($"{cursor.name} -> {name}:\tOnPress");
            this.NewTween()
                .SetDuration(0.2f)
                .LocalScaleRatio(0.9f)
                .SpriteRGBRatio(0.5f);
        }
        private void UICursorable_OnRelease(AUICursor cursor, bool withinTrigger)
        {
            Debug.Log($"{cursor.name} -> {name}:\tOnRelease, {withinTrigger}");
            this.NewTween()
                .SetDuration(0.2f)
                .LocalScaleRatio(0.9f.Inv())
                .SpriteRGBRatio(0.5f.Inv());
        }
        private void UICursorable_OnHold(AUICursor cursor)
        {
            Debug.Log($"{cursor.name} -> {name}:\tOnHold");

        }

        protected override void PlayAwake()
        {
            base.PlayAwake();
            Get<UICursorable>().OnGainHighlight.SubscribeAuto(this, UICursorable_OnGainHighlight);
            Get<UICursorable>().OnLoseHighlight.SubscribeAuto(this, UICursorable_OnLoseHighlight);
            Get<UICursorable>().OnPress.SubscribeAuto(this, UICursorable_OnPress);
            Get<UICursorable>().OnRelease.SubscribeAuto(this, UICursorable_OnRelease);
            Get<UICursorable>().OnHold.SubscribeAuto(this, UICursorable_OnHold);
        }
    }
}