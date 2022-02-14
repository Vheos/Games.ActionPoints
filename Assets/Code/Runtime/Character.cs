namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;

    public class Character : ABaseComponent
    {
        // Inspector
        [field: SerializeField, Range(0, 10)] public int MaxActionPoints { get; private set; }
        [field: SerializeField, Range(-1f, 1f)] public float ActionSpeed { get; private set; }
        [field: SerializeField] public Action[] StartingActions { get; private set; }
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

        private void ActionTargeter_OnChangeTargetable(ActionTargetable from, ActionTargetable to, Action action)
        { Debug.Log($"{name} has changed target: {(from != null ? from.name : "null")}   ---{{{action.name}}}--->   {(to != null ? to.name : "null")}"); }
        private void ActionTargetable_OnGainTargeting(ActionTargeter targeter,  Action action, bool isFirst)
        { Debug.Log($"{name} has gained targeting: {(targeter != null ? targeter.name : "null")}   /   {action.name}   /   {isFirst}"); }
        private void ActionTargetable_OnLoseTargeting(ActionTargeter targeter, Action action, bool isLast)
        { Debug.Log($"{name} has lost targeting: {(targeter != null ? targeter.name : "null")}   /   {action.name}   /   {isLast}"); }

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();

            _actionUI = Instantiate(SettingsManager.Prefabs.ActionUI);
            _actionUI.Initialize(Get<Actionable>(), () => Get<Collider>().LocalBounds().ToRect().Scale(this));

            if (Has<Actionable>())
            {

                Get<Updatable>().OnUpdate.SubEnableDisable(this, () => Get<Actionable>().ActionProgress += Time.deltaTime * ActionSpeed);
                Get<Actionable>().OnOverflowActionProgress.SubEnableDisable(this, t => Get<Actionable>().FocusProgress += t);
                //Get<Updatable>().OnUpdate.SubEnableDisable(this, () => Get<Actionable>().ActionProgress = ActionProgress, () => Get<Actionable>().FocusProgress = FocusProgress);

                Get<Actionable>().MaxActionPoints = MaxActionPoints;
                Get<Actionable>().TryChangeActions(null, StartingActions);
            }

            Get<Selectable>().OnGainSelection.SubEnableDisable(this, Selectable_OnGainHighlight);
            Get<Selectable>().OnLoseSelection.SubEnableDisable(this, Selectable_OnLoseHighlight);
            Get<Selectable>().OnPress.SubEnableDisable(this, Selectable_OnPress);
            Get<Selectable>().OnRelease.SubEnableDisable(this, Selectable_OnRelease);
            Get<Selectable>().OnHold.SubEnableDisable(this, Selectable_OnHold);

            Get<Targetable>().OnGainTargeting.SubEnableDisable(this, Targetable_OnGainTargeting);
            Get<Targetable>().OnLoseTargeting.SubEnableDisable(this, Targetable_OnLoseTargeting);

            //Get<Equiper>().OnChangeEquipable.SubscribeAuto(this, (from, to) =>
            //    Debug.Log($"Equiper {name}: {(from != null ? from.name : "null")} -> {(to != null ? to.name : "null")}"));


            Get<Equiper>().OnChangeEquipable.SubEnableDisable(this, Equiper_OnChangeEquipable);
            Get<Woundable>().MaxWounds.Set(() => MaxActionPoints);
            Get<Actionable>().LockedMaxActionPoints.Set(() => Get<Woundable>().Wounds); 


           // Get<ActionTargeter>().OnChangeTargetable.SubEnableDisable(this, ActionTargeter_OnChangeTargetable);
           // Get<ActionTargetable>().OnGainTargeting.SubEnableDisable(this, ActionTargetable_OnGainTargeting);
           // Get<ActionTargetable>().OnLoseTargeting.SubEnableDisable(this, ActionTargetable_OnLoseTargeting);





        }
    }
}