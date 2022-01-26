namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Vheos.Tools.Extensions.UnityObjects;
    using UnityEngine.InputSystem;
    using Vheos.Tools.Extensions.Math;
    using Vheos.Tools.Extensions.General;

    public class Character : ABaseComponent
    {
        private void UICursorable_OnGainHighlight(UICursor cursor, bool isFirst)
        {
            //Debug.Log($"{cursor.name} -> {name}:\tOnGainHighlight, {isFirst}");
            if (isFirst)
                this.NewTween()
                    .SetDuration(0.2f)
                    .LocalScaleRatio(1.2f);
        }
        private void UICursorable_OnLoseHighlight(UICursor cursor, bool isLast)
        {
            //Debug.Log($"{cursor.name} -> {name}:\tOnLoseHighlight, {isLast}");
            if (isLast)
                this.NewTween()
                    .SetDuration(0.2f)
                    .LocalScaleRatio(1.2f.Inv());
        }
        private void UICursorable_OnPress(UICursor cursor)
        {
            //Debug.Log($"{cursor.name} -> {name}:\tOnPress");
            this.NewTween()
                .SetDuration(0.2f)
                .LocalScaleRatio(0.9f)
                .SpriteRGBRatio(0.5f);

            cursor.As<UICursor>().Player.TargetingLine.Show(Get<Targeter>(), this.transform, cursor.transform);
        }
        private void UICursorable_OnRelease(UICursor cursor, bool withinTrigger)
        {
            //Debug.Log($"{cursor.name} -> {name}:\tOnRelease, {withinTrigger}");
            this.NewTween()
                .SetDuration(0.2f)
                .LocalScaleRatio(0.9f.Inv())
                .SpriteRGBRatio(0.5f.Inv());

            cursor.As<UICursor>().Player.TargetingLine.Hide();
        }
        private void UICursorable_OnHold(UICursor cursor)
        {
            //Debug.Log($"{cursor.name} -> {name}:\tOnHold");

        }
        private void Targeter_OnChangeTarget(Targetable from, Targetable to)
        {
            Debug.Log($"{(from != null ? from.name : "")} -> {(to != null ? to.name : "")}");

        }
        private void Targetable_OnGainTargeting(Targeter targeter, bool isFirst)
        {
            Debug.Log($"{name}:\tOnGainTargeting, {isFirst}");
            Get<SpriteOutline>().Show();
        }
        private void Targetable_OnLoseTargeting(Targeter targeter, bool isLast)
        {
            Debug.Log($"{name}:\tOnLoseTargeting, {isLast}");
            Get<SpriteOutline>().Hide();
        }

        protected override void PlayAwake()
        {
            base.PlayAwake();
            Get<Cursorable>().OnGainHighlight.SubscribeAuto(this, UICursorable_OnGainHighlight);
            Get<Cursorable>().OnLoseHighlight.SubscribeAuto(this, UICursorable_OnLoseHighlight);
            Get<Cursorable>().OnPress.SubscribeAuto(this, UICursorable_OnPress);
            Get<Cursorable>().OnRelease.SubscribeAuto(this, UICursorable_OnRelease);
            Get<Cursorable>().OnHold.SubscribeAuto(this, UICursorable_OnHold);

            Get<Targetable>().OnGainTargeting.SubscribeAuto(this, Targetable_OnGainTargeting);
            Get<Targetable>().OnLoseTargeting.SubscribeAuto(this, Targetable_OnLoseTargeting);

        }
    }
}