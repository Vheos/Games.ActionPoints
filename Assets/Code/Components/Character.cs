namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.General;

    public class Character : AEventSubscriber
    {
        // Inspector
        [SerializeField] protected List<Action> _Actions = new List<Action>();
        [SerializeField] [Range(1, 10)] protected int _MaxPoints = 5;
        [SerializeField] [Range(0f, 1f)] protected float _ActionSpeed = 1f;
        //[SerializeField] [Range(0f, 1f)] protected float _FocusRate = 0.5f;
        //[SerializeField] [Range(0f, 1f)] protected float _ExhaustRate = 0.5f;
        [SerializeField] [Range(0f, 100f)] protected float _BluntArmor = 0f;
        [SerializeField] [Range(0f, 100f)] protected float _SharpArmor = 0f;
        [SerializeField] protected Team.Predefined _StartingTeam;
        [SerializeField] protected Tool _StartingTool;
        [SerializeField] protected ActionAnimation.Clip _Idle;

        // Other
        public Quaternion LookAtRotation(Vector3 targetPosition)
        => Quaternion.LookRotation(transform.position.DirectionTowards(targetPosition)).Add(Quaternion.Euler(0, -90f, 0));
        public void LookAt(Transform targetTransform)
        => transform.AnimateRotation(this, LookAtRotation(targetTransform.position), 1f);
        public Team Team
        => Get<Teamable>().Team;
        public Combat Combat
        => Get<Combatable>().Combat;
        public Vector3 CombatPosition
        => Get<Combatable>().AnchorPosition;
        public Color Color
        => Team != null ? Team.Color : Color.white;
        public ActionAnimator ActionAnimator
        => Get<ActionAnimator>();
        public Transform HandTransform
        => Get<ActionAnimator>().HandTransform;
        public Tool Tool
        { get; private set; }
        public void Equip(Tool tool)
        {
            if (tool == null)
                return;

            Tool = tool;
            Tool.GetEquippedBy(this);
            ActionAnimator.Animate(Tool.Idle);
        }
        public void Unequip()
        {
            if (Tool == null)
                return;

            Tool.GetUnequipped();
            Tool = null;
        }
        public ActionAnimation.Clip Idle
        => Tool != null ? Tool.Idle : _Idle;

        // Private
        private UIBase _ui;
        private void ShowTargetingLine(CursorManager.Button button, Vector3 position)
        => _ui.TargetingLine.ShowAndFollowCursor(transform);
        private void TryToggleCombatWithTarget(CursorManager.Button button, Vector3 position)
        {
            _ui.TargetingLine.Hide();
            if (_ui.TargetingLine.Target.TryNonNull(out var target)
            && target.TryGetComponent<Combatable>(out var targetCombatable))
                if (targetCombatable.IsInCombat)
                    targetCombatable.TryLeaveCombat();
                else
                    Get<Combatable>().TryStartCombatWith(targetCombatable);
        }
        private void UpdateAnimatorSpeed(Vector3 from, Vector3 to)
        => Get<Animator>().SetFloat("Speed", from.DistanceTo(to) / Time.deltaTime);
        private void ResetAnimatorSpeed()
        => Get<Animator>().SetFloat("Speed", 0f);
        private void UpdateColors(Team from, Team to)
        {
            Color color = to != null ? to.Color : Color.white;
            Get<SpriteRenderer>().color = color;
            Get<SpriteOutline>().Color = color;
        }
        private void OnCombatChanged(Combat current)
        {
            bool isInCombat = current != null;
            if (isInCombat)
            {
                _ui.PointsBar.Show();
                _ui.Wheel.Show();
            }
            else
            {
                _ui.PointsBar.Hide();
                _ui.Wheel.Hide();
            }
            Get<Animator>().SetBool("IsInCombat", isInCombat);
        }
        private void OnDamageReceived(float damage, bool isWound)
        => _ui.PopupHandler.PopDamage(transform.position, damage, isWound);
        private void OnHasDied()
        {
            Get<ActionAnimator>().Stop();
            Get<ActionTargetable>().LoseTargeting();

            Get<Combatable>().TryLeaveCombat();
            Get<Combatable>().enabled = false;
            Get<Teamable>().TryLeaveTeam();
            Get<Teamable>().enabled = false;

            transform.AnimateLocalRotation(this, transform.localRotation.eulerAngles.NewZ(180f), 1f, null, QAnimator.Curve.QurveInverted);
        }
        private void OnTargeterChange(ABaseComponent from, ABaseComponent to)
        {
            if (to == null)
                Get<SpriteOutline>().Hide();
            else
                Get<SpriteOutline>().Show();
        }

        // Playable
        protected override void AutoSubscribeToEvents()
        {
            base.AutoSubscribeToEvents();
            SubscribeTo(Get<Mousable>().OnPress, ShowTargetingLine);
            SubscribeTo(Get<Mousable>().OnRelease, TryToggleCombatWithTarget);
            SubscribeTo(Get<Movable>().OnMoved, UpdateAnimatorSpeed);
            SubscribeTo(Get<Movable>().OnStoppedMoving, ResetAnimatorSpeed);
            SubscribeTo(Get<Teamable>().OnTeamChanged, UpdateColors);
            SubscribeTo(Get<Combatable>().OnCombatChanged, OnCombatChanged);
            SubscribeTo(Get<Woundable>().OnDamageReceived, OnDamageReceived);
            SubscribeTo(Get<Woundable>().OnHasDied, OnHasDied);
            SubscribeTo(Get<ActionTargetable>().OnTargeterChange, OnTargeterChange);
        }
        protected void DefineComponentInputs()
        {
            Get<Actionable>().MaxActionPoints.Set(() => _MaxPoints);
            Get<Actionable>().ActionSpeed.Set(() => _ActionSpeed);
            Get<Actionable>().LockedMaxActionPoints.Set(() => Get<Woundable>().WoundsCount);
            Get<Actionable>().Actions.Set(() => _Actions);
            Get<Woundable>().MaxWounds.Set(() => Get<Actionable>().MaxActionPoints);
            Get<Woundable>().BluntArmor.Set(() => _BluntArmor);
            Get<Woundable>().SharpArmor.Set(() => _SharpArmor);
            Get<Teamable>().StartingTeam.Set(() => _StartingTeam switch
            {
                Team.Predefined.Players => Team.Players,
                Team.Predefined.Enemies => Team.Enemies,
                _ => null,
            });
        }
        protected override void PlayAwake()
        {
            base.PlayAwake();
            DefineComponentInputs();
            _ui = UIManager.HierarchyRoot.CreateChildComponent<UIBase>(UIManager.Settings.Prefab.Base);
            _ui.Initialize(this);
        }
        protected override void PlayStart()
        {
            base.PlayStart();
            Equip(_StartingTool);
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