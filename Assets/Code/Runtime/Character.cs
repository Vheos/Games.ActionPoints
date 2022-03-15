namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;

    public class Character : ABaseComponent
    {
        // Inspector
        [field: SerializeField, Range(-5f, +5f)] public float ActionProgress { get; private set; }
        [field: SerializeField, Range(-5f, +5f)] public float FocusProgress { get; private set; }

        // Privates
        private ActionUI _actionUI;
        private void Selectable_OnGainHighlight(Selecter selecter, bool isFirst)
        {
        }
        private void Selectable_OnLoseHighlight(Selecter selecter, bool isLast)
        {
        }
        private void TrySetOwner(Selecter selecter)
        {

        }
        private void Selectable_OnRelease(Selecter selecter, bool isFullClick)
        {
            if (isFullClick)
            {
                if (this.TrySetPlayerOwnerIfNull(selecter.Get<Player>()))
                    return;

                ActionPhase phase = this.IsInCombat() ? ActionPhase.Combat : ActionPhase.Camp;
                _actionUI.ButtonWheels[phase].Get<Expandable>().Toggle();
                //_actionUI.PointsBar.Get<Expandable>().Toggle();
            }
        }
        private void Highlightable_OnGainHighlight(bool isFirst)
        {
            if (isFirst)
                Get<SpriteOutline>().Show();
        }
        private void Highlightable_OnLoseHighlight(bool isLast)
        {
            if (isLast)
                Get<SpriteOutline>().Hide();
        }
        private void Equiper_OnChangeEquipable(Equipable from, Equipable to)
        {
            var previousEquipment = from != null ? from.Get<Equipment>() : null;
            var currentEquipment = to != null ? to.Get<Equipment>() : null;
            if (previousEquipment != null)
                previousEquipment.NewTween()
                    .SetDuration(0.4f)
                    .Position(transform.position + NewUtility.RandomPointOnCircle().Abs().Append());
            if (currentEquipment != null)
                currentEquipment.NewTween()
                    .SetDuration(0.4f)
                    .Position(transform.position);

            var previousActions = from != null ? previousEquipment.Actions : null;
            var currentActions = to != null ? currentEquipment.Actions : null;
            Get<Actionable>().TryChangeActions(previousActions, currentActions);
            Get<Actionable>().MaxActionPoints += (currentEquipment != null ? currentEquipment.MaxActionPoints : 0)
                                               - (previousEquipment != null ? previousEquipment.MaxActionPoints : 0);
        }
        private void Combatable_OnChangeCombat(Combat from, Combat to)
        {
            if (to == null)
                _actionUI.CollapseButtonWheels();
            else
                _actionUI.ExpandButtonWheel(ActionPhase.Combat);
        }
        private void PlayerOwnable_OnChangePlayer(Player from, Player to)
        {
            Get<SpriteRenderer>().color = to.Color;
        }

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();

            _actionUI = Instantiate(SettingsManager.Prefabs.ActionUI);
            _actionUI.Initialize(Get<Actionable>(), () => Get<Collider>().LocalBounds().ToRect().Scale(this));
            _actionUI.Points.Get<Expandable>().TryExpand();
            //_actionUI.ButtonWheels[ActionPhase.Camp].Get<Expandable>().TryExpand();

            Get<Selectable>().OnGainSelection.SubEnableDisable(this, Selectable_OnGainHighlight);
            Get<Selectable>().OnLoseSelection.SubEnableDisable(this, Selectable_OnLoseHighlight);

            Get<Selectable>().OnRelease.SubEnableDisable(this, Selectable_OnRelease);

            Get<Highlightable>().OnGainHighlight.SubEnableDisable(this, Highlightable_OnGainHighlight);
            Get<Highlightable>().OnLoseHighlight.SubEnableDisable(this, Highlightable_OnLoseHighlight);

            //Get<Equiper>().OnChangeEquipable.SubscribeAuto(this, (from, to) =>
            //    Debug.Log($"Equiper {name}: {(from != null ? from.name : "null")} -> {(to != null ? to.name : "null")}"));


            Get<Equiper>().OnChangeEquipable.SubEnableDisable(this, Equiper_OnChangeEquipable);
            Get<Combatable>().OnChangeCombat.SubEnableDisable(this, Combatable_OnChangeCombat);

            if (TryGet(out PlayerOwnable playerOwnable))
            {
                Get<Selectable>().OnPress.SubEnableDisable(this, TrySetOwner);
                playerOwnable.OnChangeOwner.SubEnableDisable(this, PlayerOwnable_OnChangePlayer);
            }

            // Get<ActionTargeter>().OnChangeTargetable.SubEnableDisable(this, ActionTargeter_OnChangeTargetable);
            // Get<ActionTargetable>().OnGainTargeting.SubEnableDisable(this, ActionTargetable_OnGainTargeting);
            // Get<ActionTargetable>().OnLoseTargeting.SubEnableDisable(this, ActionTargetable_OnLoseTargeting);
        }
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