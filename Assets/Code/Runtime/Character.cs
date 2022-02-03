namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;

    public class Character : ABaseComponent
    {
        // Inspector
        [SerializeField] [Range(0, 10)] protected int _MaxActionPoints;
        [SerializeField] [Range(-1f, 1f)] protected float _ActionSpeed;
        [SerializeField] protected ActionUI _ActionUIPrefab;
        [SerializeField] protected Action[] _StartingActions;

        // Privates
        private ActionUI _actionUI;
        private void Selectable_OnGainHighlight(Selecter selecter, bool isFirst)
        {
        }
        private void Selectable_OnLoseHighlight(Selecter selecter, bool isLast)
        {
        }
        private void Selectable_OnPress(Selecter selecter)
        { }
        private void Selectable_OnRelease(Selecter selecter, bool withinTrigger)
        {
            if (withinTrigger)
            {
                _actionUI.ButtonsWheel.Get<Expandable>().Toggle();
                _actionUI.PointsBar.Get<Expandable>().Toggle();
            }
        }

        /*
        private void Selectable_OnPress(Selecter selecter)
        {
            //Debug.Log($"{selecter.name} -> {name}:\tOnPress");
            this.NewTween()
                .SetDuration(0.2f)
                .LocalScaleRatio(0.9f)
                .SpriteRGBRatio(0.75f);

            selecter.Get<Player>().TargetingLine.Show(Get<Targeter>(), this.transform, selecter.Get<Player>().Cursor.transform);
        }
        private void Selectable_OnRelease(Selecter selecter, bool withinTrigger)
        {
            //Debug.Log($"{selecter.name} -> {name}:\tOnRelease, {withinTrigger}");
            this.NewTween()
                .SetDuration(0.2f)
                .LocalScaleRatio(0.9f.Inv())
                .SpriteRGBRatio(0.75f.Inv());

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
        */
        private void Selectable_OnHold(Selecter selecter)
        {
            //Debug.Log($"{selecter.name} -> {name}:\tOnHold");

        }
        private void Targetable_OnGainTargeting(Targeter targeter, bool isFirst)
        {
            if (isFirst)
                Get<SpriteOutline>().Show();
        }
        private void Targetable_OnLoseTargeting(Targeter targeter, bool isLast)
        {
            if (isLast)
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

            //Get<Equiper>().OnChangeEquipable.SubscribeAuto(this, (from, to) =>
            //    Debug.Log($"Equiper {name}: {(from != null ? from.name : "null")} -> {(to != null ? to.name : "null")}"));

            if (Has<Actionable>())
            {
                Get<Actionable>().MaxActionPoints.Set(() => _MaxActionPoints);
                Get<Actionable>().TryAddActions(_StartingActions);

                Get<Updatable>().OnUpdate.SubscribeAuto(this, () => Get<Actionable>().ActionProgress += Time.deltaTime * _ActionSpeed);
                Get<Actionable>().OnOverflowActionProgress.SubscribeAuto(this, t => Get<Actionable>().FocusProgress += t);
            }

            if (_ActionUIPrefab != null)
            {
                _actionUI = Instantiate(_ActionUIPrefab);
                _actionUI.Initialize(Get<Actionable>(), () => Get<Collider>().LocalBounds().ToRect().Scale(this));
            }
        }
    }
}