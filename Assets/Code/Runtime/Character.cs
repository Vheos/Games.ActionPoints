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
        // Inspector
        [SerializeField] [Range(0, 10)] protected int _MaxActionPoints;
        [SerializeField] [Range(-1f, 1f)] protected float _ActionSpeed;

        // Privates
        private void Selectable_OnGainHighlight(Selecter selecter, bool isFirst)
        {
            //Debug.Log($"{selecter.name} -> {name}:\tOnGainHighlight, {isFirst}");
            if (isFirst)
                this.NewTween()
                    .SetDuration(0.2f)
                    .LocalScaleRatio(1.2f);
        }
        private void Selectable_OnLoseHighlight(Selecter selecter, bool isLast)
        {
            //Debug.Log($"{selecter.name} -> {name}:\tOnLoseHighlight, {isLast}");
            if (isLast)
                this.NewTween()
                    .SetDuration(0.2f)
                    .LocalScaleRatio(1.2f.Inv());
        }
        private void Selectable_OnPress(Selecter selecter)
        {
            //Debug.Log($"{selecter.name} -> {name}:\tOnPress");
            this.NewTween()
                .SetDuration(0.2f)
                .LocalScaleRatio(0.9f)
                .SpriteRGBRatio(0.5f);

            selecter.Get<Player>().TargetingLine.Show(Get<Targeter>(), this.transform, selecter.Get<Player>().Cursor.transform);
        }
        private void Selectable_OnRelease(Selecter selecter, bool withinTrigger)
        {
            //Debug.Log($"{selecter.name} -> {name}:\tOnRelease, {withinTrigger}");
            this.NewTween()
                .SetDuration(0.2f)
                .LocalScaleRatio(0.9f.Inv())
                .SpriteRGBRatio(0.5f.Inv());

            if (Get<Targeter>().IsTargetingAny
            && !Get<Targeter>().IsTargeting(Get<Targetable>()))
            {
                this.NewTween()
                    .SetDuration(1f)
                    .Position(this.transform.position.Lerp(Get<Targeter>().Targetable.transform.position, 0.5f));
            }

            if (withinTrigger
            && selecter.TryGet(out Player player)
            && TryGet(out PlayerOwnable playerOwnable)
            && playerOwnable.Owner == null)
                playerOwnable.Owner = player;

            selecter.Get<Player>().TargetingLine.Hide();
        }
        private void Selectable_OnHold(Selecter selecter)
        {
            //Debug.Log($"{selecter.name} -> {name}:\tOnHold");

        }
        private void Targeter_OnChangeTarget(Targetable from, Targetable to)
        {
            //Debug.Log($"{(from != null ? from.name : "")} -> {(to != null ? to.name : "")}");

        }
        private void Targetable_OnGainTargeting(Targeter targeter, bool isFirst)
        {
            // Debug.Log($"{name}:\tOnGainTargeting, {isFirst}");
            if (targeter.TryGet(out PlayerOwnable playerOwnable)
            && playerOwnable.Owner != null)
                Get<SpriteOutline>().Color = playerOwnable.Owner.Color;

            Get<SpriteOutline>().Show();
        }
        private void Targetable_OnLoseTargeting(Targeter targeter, bool isLast)
        {
            //Debug.Log($"{name}:\tOnLoseTargeting, {isLast}");
            Get<SpriteOutline>().Hide();
        }

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();

            Get<Selectable>().OnGainSelection.SubscribeAuto(this, Selectable_OnGainHighlight);
            Get<Selectable>().OnLoseSelection.SubscribeAuto(this, Selectable_OnLoseHighlight);
            Get<Selectable>().OnPress.SubscribeAuto(this, Selectable_OnPress);
            Get<Selectable>().OnRelease.SubscribeAuto(this, Selectable_OnRelease);
            Get<Selectable>().OnHold.SubscribeAuto(this, Selectable_OnHold);

            Get<Targetable>().OnGainTargeting.SubscribeAuto(this, Targetable_OnGainTargeting);
            Get<Targetable>().OnLoseTargeting.SubscribeAuto(this, Targetable_OnLoseTargeting);

            if (Has<Actionable>())
            {
                Get<Actionable>().MaxActionPoints.Set(() => _MaxActionPoints);
                Get<Updatable>().OnUpdate.SubscribeAuto(this, () => Get<Actionable>().ActionProgress += Time.deltaTime * _ActionSpeed);
                Get<Actionable>().OnOverflowActionProgress.SubscribeAuto(this, t => Get<Actionable>().FocusProgress += t);
            }

        }
    }
}