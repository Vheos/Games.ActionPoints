namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.General;
    using System.Collections.Generic;

    public class Character : AEventSubscriber
    {
        // Inspector
        [SerializeField] protected Action[] _Actions = new Action[1];
        [SerializeField] [Range(1, 10)] protected int _MaxPoints = 5;
        [SerializeField] [Range(0f, 1f)] protected float _ActionSpeed = 1f;
        //[SerializeField] [Range(0f, 1f)] protected float _FocusRate = 0.5f;
        //[SerializeField] [Range(0f, 1f)] protected float _ExhaustRate = 0.5f;
        [SerializeField] [Range(0f, 100f)] protected float _BluntArmor = 0f;
        [SerializeField] [Range(0f, 100f)] protected float _SharpArmor = 0f;
        [SerializeField] protected Team.Predefined _StartingTeam = Team.Predefined.None;
        [SerializeField] protected Equipable[] _StartingEquipment = new Equipable[0];

        // Other
        public Quaternion LookAtRotation(Vector3 targetPosition)
        => Quaternion.LookRotation(transform.position.DirectionTowards(targetPosition)).Add(Quaternion.Euler(0, -90f, 0));
        public Team Team
        => Get<Teamable>().Team;
        public Combat Combat
        => Get<Combatable>().Combat;
        public Vector3 CombatPosition
        => Get<Combatable>().AnchorPosition;
        public Color Color
        => Team != null ? Team.Color : Color.white;
        public Transform HandTransform
        => Get<ActionAnimator>().HandTransform;
        public Transform GetEquipableAttachTransform(Equipable.Slot slot)
        => slot switch
        {
            Equipable.Slot.Hand => Get<ActionAnimator>().HandTransform,
            _ => null,
        };
        public bool IsUnsheathed;

        // Private
        private UIBase _ui;
        private Action _contextualAction;
        private void OnUpdate()
        {
            if (Get<Combatable>().IsInCombat)
                Get<Actionable>().ActionProgress += Time.deltaTime * _ActionSpeed * ActionManager.GlobalSpeedScale;
        }
        private void OnPress(UIManager.ButtonFunction function)
        => _ui.TargetingLine.ShowAndFollowCursor(transform, TargetingLine_OnChangeTarget);
        private void TargetingLine_OnChangeTarget(Targetable from, Targetable to)
        {
            if (to == null)
            {
                _contextualAction = null;
                Get<Targeter>().Target = null;
                return;
            }

            foreach (var action in ActionManager.Common.AllActions)
                if (action.CanTarget(Get<Targeter>(), to))
                    _contextualAction = action;

            if (_contextualAction == null)
                return;

            Get<Targeter>().Target = to;
            Get<ActionAnimator>().Animate(_contextualAction, ActionAnimation.Type.Target);
        }
        private void OnRelease(UIManager.ButtonFunction function, bool isClick)
        {
            if (isClick)
            {
                _ui.Wheel.Toggle();
                if (_ui.Wheel.IsExpanded)
                    TryUnsheathe();
                else
                    TrySheathe();
            }
            else if (_contextualAction != null)
            {
                Get<Actionable>().Use(_contextualAction, Get<Targeter>().Target);
                Get<ActionAnimator>().Animate(_contextualAction, ActionAnimation.Type.UseThenIdle);
            }

            Get<Targeter>().Target = null;
            _ui.TargetingLine.Hide();
        }
        private void SetAnimatorMoveSpeed(float speed)
        => Get<Animator>().SetFloat("Speed", speed);
        private void UpdateColors(Team from, Team to)
        {
            Color color = to != null ? to.Color : Color.white;
            Get<SpriteRenderer>().color = color;
            Get<SpriteOutline>().Color = color;
        }
        private void OnCombatChanged(Combat current)
        {
            bool isInCombat = current != null;
            Get<Animator>().SetBool("IsInCombat", isInCombat);

            if (isInCombat)
            {
                _ui.PointsBar.Show();
                if (!Has<AIController>())
                    _ui.Wheel.Show();
                TryUnsheathe();

                foreach (var ally in Get<Teamable>().Allies)
                    if (ally.TryGet<Combatable>(out var allyCombatable))
                        allyCombatable.TryJoinCombat(current);
            }
            else
            {
                _ui.PointsBar.Hide();
                _ui.Wheel.Hide();
                TrySheathe();

                Get<Actionable>().ActionProgress = 0;
                Get<Actionable>().FocusProgress = 0;
                Get<Targetable>().ClearAllTargeting();
            }
        }
        private void OnDamageReceived(float damage, bool isWound)
        => _ui.PopupHandler.PopDamage(transform.position, damage, isWound);
        private void OnHasDied()
        {
            Get<ActionAnimator>().Stop();
            Get<Combatable>().TryLeaveCombat();
            Get<Teamable>().TryLeaveTeam();

            transform.AnimateLocalRotation(this, transform.localRotation.eulerAngles.NewZ(180f), 1f, null, QAnimator.Curve.Qurve);
        }
        private void OnGainTargeting(Targeter targeter, bool isFirst)
        {
            if (isFirst)
                Get<SpriteOutline>().Show();
        }
        private void OnLoseTargeting(Targeter targeter, bool isLast)
        {
            if (isLast)
                Get<SpriteOutline>().Hide();
        }
        private void OnChangeTarget(Targetable from, Targetable to)
        {
            if (to != null)
                Get<RotateTowards>().SetTarget(to.transform, true);
        }
        private void OnChangeEquipable(Equipable.Slot slot, Equipable from, Equipable to)
        {
            if (from != null && from.TryGet<Tool>(out var fromTool))
                fromTool.DetachTo(to.transform);
            if (to != null && to.TryGet<Tool>(out var toTool))
            {
                var startAnim = IsUnsheathed ? toTool.AnimationSet.Idle : toTool.AnimationSet.Sheathe;
                    Get<ActionAnimator>().Animate(startAnim, true);
                toTool.AttachTo(GetEquipableAttachTransform(slot));
            }
        }
        private void TryUnsheathe()
        {
            if (IsUnsheathed || !this.TryGetTool(out var tool))
                return;

            IsUnsheathed = true;
            Get<ActionAnimator>().Animate(tool.AnimationSet.Sheathe, true);
            Get<ActionAnimator>().Animate(tool.AnimationSet.Idle);
        }
        private void TrySheathe()
        {
            if (!IsUnsheathed || !this.TryGetTool(out var tool))
                return;

            IsUnsheathed = false;
            Get<ActionAnimator>().Animate(tool.AnimationSet.Sheathe);
        }

        // Playable
        protected void DefineComponentInputs()
        {
            Get<Actionable>().MaxActionPoints.Set(() => _MaxPoints);
            Get<Actionable>().LockedMaxActionPoints.Set(() => Get<Woundable>().WoundsCount);
            Get<Woundable>().MaxWounds.Set(() => Get<Actionable>().MaxActionPoints);
            Get<Woundable>().BluntArmor.Set(() => _BluntArmor);
            Get<Woundable>().SharpArmor.Set(() => _SharpArmor);
            Get<Teamable>().StartingTeam.Set(() => _StartingTeam switch
            {
                Team.Predefined.Players => Team.Players,
                Team.Predefined.Enemies => Team.AI,
                _ => null,
            });
            Get<Equiper>().AttachTransformsBySlot.Set((slot) => slot switch
            {
                Equipable.Slot.Hand => Get<ActionAnimator>().HandTransform,
                _ => null
            }); ;
        }
        protected override void DefineAutoSubscriptions()
        {
            base.DefineAutoSubscriptions();
            SubscribeTo(Get<Selectable>().OnPress, OnPress);
            SubscribeTo(Get<Selectable>().OnRelease, OnRelease);
            SubscribeTo(Get<Movable>().OnMove, (from, to) => SetAnimatorMoveSpeed(from.DistanceTo(to) / Time.deltaTime));
            SubscribeTo(Get<Movable>().OnStop, () => SetAnimatorMoveSpeed(0f));
            SubscribeTo(Get<Teamable>().OnChangeTeam, UpdateColors);
            SubscribeTo(Get<Combatable>().OnChangeCombat, OnCombatChanged);
            SubscribeTo(Get<Woundable>().OnReceiveDamage, OnDamageReceived);
            SubscribeTo(Get<Woundable>().OnDie, OnHasDied);
            SubscribeTo(Get<Targetable>().OnGainTargeting, OnGainTargeting);
            SubscribeTo(Get<Targetable>().OnLoseTargeting, OnLoseTargeting);
            SubscribeTo(Get<Targeter>().OnChangeTarget, OnChangeTarget);
            SubscribeTo(Get<Equiper>().OnChangeEquipable, OnChangeEquipable);
            SubscribeTo(Get<Updatable>().OnUpdate, OnUpdate);
        }
        protected override void PlayAwake()
        {
            base.PlayAwake();
            DefineComponentInputs();
            Get<Actionable>().TryAddActions(_Actions);
            _ui = UIManager.HierarchyRoot.CreateChildComponent<UIBase>(UIManager.Settings.Prefab.Base);
            _ui.Initialize(this);
        }
        protected override void PlayStart()
        {
            base.PlayStart();
            foreach (var equipable in _StartingEquipment)
                Get<Equiper>().TryEquip(equipable);
        }
        protected override void PlayDestroy()
        {
            base.PlayDestroy();
            if (_ui != null)
                _ui.DestroyObject();
        }

#if UNITY_EDITOR
        public override void EditAwake()
        {
            base.EditAwake();
            //Get<SpriteRenderer>().color = StartingTeam != null ? StartingTeam.Color : Color.white;
        }
#endif
    }
}