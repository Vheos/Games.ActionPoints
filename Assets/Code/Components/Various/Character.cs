namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.General;

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
        [SerializeField] protected ActionAnimation.Clip _Idle = new ActionAnimation.Clip();

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
        public Tool Tool
        => Get<Equiper>().TryGetEquiped(Equipable.Slot.Hand, out var equipable)
        && equipable.TryGet<Tool>(out var tool)
         ? tool : null;
        public ActionAnimation.Clip Idle
        => Tool != null ? Tool.Idle : _Idle;

        // Private
        private UIBase _ui;
        private Action _contextualAction;
        private void OnPress(UIManager.ButtonFunction function)
        => _ui.TargetingLine.ShowAndFollowCursor(transform, TargetingLine_OnChangeTarget);
        private void TargetingLine_OnChangeTarget(Targetable from, Targetable to)
        {
            Get<Targeter>().Target = to;
            _contextualAction = null;
            if (to == null || to == Get<Targetable>())
                return;

            if (to.Has<Equipable>())
                _contextualAction = ActionManager.Common.Equip;
            else if (to.Has<Combatable>())
                _contextualAction = ActionManager.Common.StartCombat;

            if (_contextualAction != null)
                Get<ActionAnimator>().TryAnimate(_contextualAction, ActionAnimation.Type.Charge);
        }
        private void OnRelease(UIManager.ButtonFunction function, bool isClick)
        {
            if (isClick)
                _ui.Wheel.Toggle();
            else if (_contextualAction != null)
            {
                Get<Actionable>().Use(_contextualAction, Get<Targeter>().Target);
                Get<ActionAnimator>().TryAnimate(_contextualAction, ActionAnimation.Type.Release);
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

                foreach (var ally in Get<Teamable>().Allies)
                    if (ally.TryGet<Combatable>(out var allyCombatable))
                        allyCombatable.TryJoinCombat(current);
            }
            else
            {
                _ui.PointsBar.Hide();
                _ui.Wheel.Hide();

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
        private void OnUpdate()
        {
            if (Get<Combatable>().IsInCombat)
                Get<Actionable>().ActionProgress += Time.deltaTime * _ActionSpeed * ActionManager.GlobalSpeedScale;
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
                Get<Equiper>().Equip(equipable);
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