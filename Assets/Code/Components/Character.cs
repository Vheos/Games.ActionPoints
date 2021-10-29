// change AMousable to component (from abstract class)

namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.General;

    public class Character : ABaseComponent
    {
        // Inspector
        [SerializeField] protected List<Action> _Actions = new List<Action>();
        [SerializeField] [Range(1, 10)] protected int _RawMaxPoints = 5;
        [SerializeField] [Range(0f, 1f)] protected float _ActionSpeed = 1f;
        [SerializeField] [Range(0f, 1f)] protected float _FocusSpeed = 0.5f;
        [SerializeField] [Range(0f, 1f)] protected float _ExhaustSpeed = 0.5f;
        [SerializeField] [Range(0f, 100f)] protected float _BluntArmor = 0f;
        [SerializeField] [Range(0f, 100f)] protected float _SharpArmor = 0f;
        [SerializeField] protected Tool _StartingTool;
        [SerializeField] protected ActionAnimation.Clip _Idle;

        // Cached components
        protected override System.Type[] ComponentsTypesToCache => new[]
        {
            typeof(SpriteRenderer),
            typeof(SpriteOutline),
            typeof(Animator),
            typeof(ActionAnimator),
            typeof(Teamable),
        };

        // Publics (action)
        public float ActionProgress
        { get; private set; }
        public int ActionPointsCount
        => ActionProgress.RoundTowardsZero();
        public void ChangeActionPoints(int diff)
        => ActionProgress = ActionProgress.Add(diff).Clamp(-MaxActionPoints, +MaxActionPoints);
        public int RawMaxActionPoints
        => _RawMaxPoints;
        public int MaxActionPoints
        => _RawMaxPoints - WoundsCount;
        public bool IsExhausted
        => ActionProgress < 0;
        // Focus
        public float FocusProgress
        { get; private set; }
        public int FocusPointsCount
        => FocusProgress.RoundTowardsZero();
        public void ChangeFocusPoints(int diff)
        => FocusProgress = FocusProgress.Add(diff).ClampMax(ActionProgress).ClampMin(0f);
        // Damage
        public float CalculateTotalDamage(float blunt, float sharp, float raw)
        {
            blunt += 0 - _BluntArmor;
            sharp *= 1 - _SharpArmor / 100f;
            blunt.SetClampMin(0);
            sharp.SetClampMin(0);
            raw.SetClampMin(0);
            return blunt + sharp + raw;
        }
        public void TakeTotalDamage(float totalDamage)
        {
            int sureWounds = totalDamage.Div(100f).RoundDown();
            float remainingDamage = totalDamage - sureWounds;
            int rolledWounds = remainingDamage.RollPercent().To01();
            int totalWounds = sureWounds + rolledWounds;

            _actionUI.PopDamage(transform.position, totalDamage, totalWounds);
            ChangeWoundsCount(totalWounds);
        }
        // Wounds
        public int WoundsCount
        { get; private set; }
        private void ChangeWoundsCount(int diff)
        => WoundsCount = WoundsCount.Add(diff).Clamp(0, _RawMaxPoints);
        // Other
        public IEnumerable<Action> Actions
        => _Actions;
        public void LookAt(Transform targetTransform)
        {
            if (targetTransform == transform)
                return;

            if (TryGetComponent<RotateTowards>(out var rotateTowards))
                rotateTowards.Target = targetTransform;
            else
                transform.rotation.SetLookRotation(transform.DirectionTowards(targetTransform));
        }
        public Team Team
        => Get<Teamable>().Team;
        public Color Color
        => Team != null ? Team.Color : Color.white;
        public Vector3 CombatPosition
        { get; private set; }
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
        private UIBase _actionUI;
        private float _previousActionProgress;
        private float _previousFocusProgress;
        private int _previousWoundsCount;
        private void UpdateProgresses()
        {
            float deltaTime = Time.deltaTime;
            ActionProgress += deltaTime * _ActionSpeed;
            if (ActionProgress > MaxActionPoints)
            {
                deltaTime = (ActionProgress - MaxActionPoints) / _ActionSpeed;
                ActionProgress = MaxActionPoints;
                FocusProgress += deltaTime * _FocusSpeed;
                FocusProgress = FocusProgress.ClampMax(ActionProgress);
            }
        }

        // Events
        private void InvokeEvents()
        {
            int previousActionsPointsCount = _previousActionProgress.RoundTowardsZero();
            int currentActionPointsCount = ActionProgress.RoundTowardsZero();
            if (previousActionsPointsCount != ActionPointsCount)
                OnActionPointsCountChanged?.Invoke(previousActionsPointsCount, currentActionPointsCount);

            int previousFocusPointsCount = _previousFocusProgress.RoundTowardsZero();
            int currentFocusPointsCount = FocusProgress.RoundTowardsZero();
            if (previousFocusPointsCount != FocusPointsCount)
                OnFocusPointsCountChanged?.Invoke(previousFocusPointsCount, currentFocusPointsCount);

            if (_previousActionProgress < 0 && ActionProgress >= 0)
                OnExhaustStateChanged?.Invoke(false);
            else if (_previousActionProgress >= 0 && ActionProgress < 0)
                OnExhaustStateChanged?.Invoke(true);

            if (_previousWoundsCount != WoundsCount)
                OnWoundsCountChanged?.Invoke(_previousWoundsCount, WoundsCount);
        }
        public event System.Action<int, int> OnActionPointsCountChanged;
        public event System.Action<int, int> OnFocusPointsCountChanged;
        public event System.Action<bool> OnExhaustStateChanged;
        public event System.Action<int, int> OnWoundsCountChanged;

        // Playable
        public override void PlayAwake()
        {
            base.PlayAwake();

            _actionUI = UIManager.HierarchyRoot.CreateChildComponent<UIBase>(UIManager.Settings.Prefab.Base);
            _actionUI.Character = this;
        }
        public override void PlayStart()
        {
            base.PlayStart();
            Equip(_StartingTool);
            CombatPosition = TryGetComponent<SnapTo>(out var snapTo) && snapTo.IsActive ? snapTo.TargetPosition : transform.position;
        }
        public override void PlayDestroy()
        {
            base.PlayDestroy();
            if (_actionUI != null)
                _actionUI.DestroyObject();
        }
        protected override void SubscribeToPlayEvents()
        {
            base.SubscribeToPlayEvents();
            Updatable.OnPlayUpdate += () =>
            {
                UpdateProgresses();
                InvokeEvents();

                _previousActionProgress = ActionProgress;
                _previousFocusProgress = FocusProgress;
                _previousWoundsCount = WoundsCount;
            };
            Mousable.OnGainHighlight += () =>
            {
                Get<SpriteOutline>().Show();
            };
            Mousable.OnPress += (button, position) =>
            {
                _actionUI.ToggleWheel();
            };
            Mousable.OnLoseHighlight += () =>
            {
                Get<SpriteOutline>().Hide();
            };
            Movable.OnMoved += (from, to) =>
            {
                Get<Animator>().SetFloat("Speed", from.DistanceTo(to) / Time.deltaTime);
            };
            Movable.OnStopped += (position) =>
            {
                Get<Animator>().SetFloat("Speed", 0f);
            };
            Get<Teamable>().OnTeamChanged += (from, to) =>
            {
                Get<SpriteRenderer>().color = to.Color;
                Get<SpriteOutline>().Color = to.Color;
            };
        }

#if UNITY_EDITOR
        public override void EditAwake()
        {
            base.EditAwake();
            if (GetComponent<Teamable>().StartingTeam.TryNonNull(out var team))
                GetComponent<SpriteRenderer>().color = team.Color;
            else
                GetComponent<SpriteRenderer>().color = Color.white;
        }
#endif
    }
}