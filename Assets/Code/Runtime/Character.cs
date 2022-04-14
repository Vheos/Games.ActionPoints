namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;
    using Vheos.Tools.Extensions.UnityObjects;

    public class Character : ABaseComponent
    {
        // Privates
        private ActionUI _actionUI;
        private void Selectable_OnGainHighlight(Selectable selectable, Selecter selecter)
        {
        }
        private void Selectable_OnLoseHighlight(Selectable selectable, Selecter selecter)
        {
        }
        private void Selectable_OnRelease(Selectable selectable, Selecter selecter, bool isFullClick)
        {
            if (isFullClick && TryGet(out PlayerOwnable playerOwnable))
                if (playerOwnable.Owner == null)
                    playerOwnable.Owner = selecter.Get<Player>();
                else
                {
                    ActionPhase phase = this.IsInCombat() ? ActionPhase.Combat : ActionPhase.Camp;
                    _actionUI.Buttons[phase].Get<Expandable>().Toggle();
                }
        }
        private void Highlightable_OnGainHighlight(Highlightable highlightable, Highlighter highlighter)
        {
            if (highlightable.IsHighlightedByMany)
                return;

            Get<SpriteOutline>().Show();
        }
        private void Highlightable_OnLoseHighlight(Highlightable highlightable, Highlighter highlighter)
        {
            if (highlightable.IsHighlighted)
                return;

            Get<SpriteOutline>().Hide();
        }
        private void Equiper_OnChangeEquipable(Equiper equiper, Equipable from, Equipable to)
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
        private void Combatable_OnChangeCombat(Combatable combatable)
        {
            if (combatable.Combat == null)
                _actionUI.CollapseButtons();
            else
            {
                _actionUI.ExpandButtons(ActionPhase.Combat);
                foreach (var ally in Get<Teamable>().Allies)
                    if (ally.TryGet(out Combatable allyCombatable))
                        allyCombatable.Combat = combatable.Combat;
            }
            Get<Animator>().SetBool(nameof(combatable.IsInCombat), combatable.IsInCombat);
        }
        private void PlayerOwnable_OnChangePlayer(Player from, Player to)
        => Get<SpriteRenderer>().color = to.Color;
        private void Woundable_OnDie()
        {
            _actionUI.Disable();
            Get<Targetable>().Disable();
            Get<Highlightable>().Disable();
            Get<SpriteOutline>().Disable();
            Get<Combatable>().Disable();

            var _mprops = Get<SpriteShadowMProps>();
            this.NewTween()
              .SetDuration(1f)
              .AddPropertyModifier(v => _mprops.OpacityDitheringRatio += v, 0f - _mprops.OpacityDitheringRatio)
              .AddEventsOnFinish(this.DestroyObject);
        }
        private void SetAnimatorSpeed(Vector3 from, Vector3 to)
        => Get<Animator>().SetFloat("Speed", from.DistanceTo(to) / Time.deltaTime);
        private void ResetAnimatorSpeed(Vector3 at)
        => Get<Animator>().SetFloat("Speed", 0f);


        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();

            _actionUI = Instantiate(SettingsManager.Prefabs.ActionUI);
            _actionUI.Initialize(Get<Actionable>(), () => Get<Collider>().LocalBounds().ToRect().Scale(this));
            _actionUI.Points.Get<Expandable>().TryExpand();
            //_actionUI.ButtonWheels[ActionPhase.Camp].Get<Expandable>().TryExpand();

            Get<Selectable>().OnGetSelected.SubEnableDisable(this, Selectable_OnGainHighlight);
            Get<Selectable>().OnGetDeselected.SubEnableDisable(this, Selectable_OnLoseHighlight);

            Get<Selectable>().OnGetReleased.SubEnableDisable(this, Selectable_OnRelease);

            Get<Highlightable>().OnGainHighlight.SubEnableDisable(this, Highlightable_OnGainHighlight);
            Get<Highlightable>().OnLoseHighlight.SubEnableDisable(this, Highlightable_OnLoseHighlight);

            //Get<Equiper>().OnChangeEquipable.SubscribeAuto(this, (from, to) =>
            //    Debug.Log($"Equiper {name}: {(from != null ? from.name : "null")} -> {(to != null ? to.name : "null")}"));


            Get<Equiper>().OnChangeEquipable.SubEnableDisable(this, Equiper_OnChangeEquipable);
            Get<Combatable>().OnChangeCombat.SubEnableDisable(this, Combatable_OnChangeCombat);

            if (TryGet(out PlayerOwnable playerOwnable))
                playerOwnable.OnChangeOwner.SubEnableDisable(this, PlayerOwnable_OnChangePlayer);


            Get<Woundable>().OnDie.SubDestroy(this, Woundable_OnDie);

            Get<SpriteShadowMProps>().Initialize();
            Get<Movable>().OnMove.SubEnableDisable(this, SetAnimatorSpeed);
            Get<Movable>().OnStop.SubEnableDisable(this, ResetAnimatorSpeed);
        }
        protected override void PlayDestroy()
        => this.StopGameObjectTweens();
    }
}