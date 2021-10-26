// change AMousable to component (from abstract class)

namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.General;

    [RequireComponent(typeof(SpriteOutline))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(TeamMember))]
    [RequireComponent(typeof(ActionAnimator))]
    public class Character : AMousableSprite
    {
        // Inspector
        public List<Action> _Actions = new List<Action>();
        [Range(1, 10)] public int _RawMaxPoints = 5;
        [Range(0f, 1f)] public float _ActionSpeed = 1f;
        [Range(0f, 1f)] public float _FocusSpeed = 0.5f;
        [Range(0f, 1f)] public float _ExhaustSpeed = 0.5f;
        [Range(0f, 100f)] public float _BluntArmor = 0f;
        [Range(0f, 100f)] public float _SharpArmor = 0f;
        public Tool _StartingTool;

        // Publics
        // Action
        public float ActionProgress
        { get; private set; }
        public int ActionPointsCount
        => ActionProgress.RoundTowardsZero();
        public void ChangeActionPoints(int diff)
        => ActionProgress = ActionProgress.Add(diff).Clamp(-MaxActionPoints, +MaxActionPoints);
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
        public void DealTotalDamage(float totalDamage)
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
        public Bounds LocalBounds
        => _mousableCollider.LocalBounds();
        public void LookAt(Transform targetTransform)
        {
            if (targetTransform == transform)
                return;

            if (TryGetComponent<RotateTowards>(out var rotateTowards))
                rotateTowards._Target = targetTransform;
            else
                transform.rotation.SetLookRotation(transform.DirectionTowards(targetTransform));
        }
        public Team Team
        => _teamMember.Team;
        public Color Color
        => Team != null ? Team._Color : Color.white;
        public Vector3 CombatPosition
        { get; private set; }
        public ActionAnimator ActionAnimator
        => GetComponent<ActionAnimator>();
        public Transform HandTransform
        => _actionAnimator.HandTransform;
        public Tool Tool
        { get; private set; }
        public void Equip(Tool tool)
        {
            if (tool == null)
                return;

            Tool = tool;
            Tool.GetEquippedBy(this);
        }
        public void Unequip()
        {
            if (Tool == null)
                return;

            Tool.GetUnequipped();
            Tool = null;
        }

        // Private
        private UIBase _actionUI;
        private float _previousActionProgress;
        private float _previousFocusProgress;
        private int _previousWoundsCount;
        private Vector3 _previousPosition;
        private void UpdateProgresses(float deltaTime)
        {
            ActionProgress += deltaTime * _ActionSpeed;
            if (ActionProgress > MaxActionPoints)
            {
                deltaTime = (ActionProgress - MaxActionPoints) / _ActionSpeed;
                ActionProgress = MaxActionPoints;
                FocusProgress += deltaTime * _FocusSpeed;
                FocusProgress = FocusProgress.ClampMax(ActionProgress);
            }
        }
        private void UpdateWalkAnimation(float deltaTime)
        {
            float speed = _previousPosition.DistanceTo(transform.position) / deltaTime;
            _animator.SetFloat("Speed", speed);
            _previousPosition = transform.position;
        }
        private void UpdateColors()
        {
            _spriteRenderer.color = Color;
            _spriteOutline._Color = Color;
        }
        // Components
        private SpriteOutline _spriteOutline;
        private TeamMember _teamMember;
        private Animator _animator;
        private ActionAnimator _actionAnimator;
        private void CacheComponents()
        {
            _animator = GetComponent<Animator>();
            _spriteOutline = GetComponent<SpriteOutline>();
            _teamMember = GetComponent<TeamMember>();
            _actionAnimator = GetComponent<ActionAnimator>();
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
        public delegate void PointsCountChanged(int previous, int current);
        public delegate void ExhaustStateChangedZero(bool isExhausted);
        public delegate void WoundsCountChanged(int previous, int current);
        public event PointsCountChanged OnActionPointsCountChanged;
        public event PointsCountChanged OnFocusPointsCountChanged;
        public event ExhaustStateChangedZero OnExhaustStateChanged;
        public event WoundsCountChanged OnWoundsCountChanged;

        // Mouse
        public override void MouseGainHighlight()
        {
            base.MouseGainHighlight();
            _spriteOutline.Show();
        }
        public override void MousePress(CursorManager.Button button, Vector3 location)
        {
            base.MousePress(button, location);
            _actionUI.ToggleWheel();
        }
        public override void MouseLoseHighlight()
        {
            base.MouseLoseHighlight();
            _spriteOutline.Hide();
        }

        // Mono
        public override void PlayAwake()
        {
            base.PlayAwake();
            CacheComponents();

            _actionUI = UIManager.HierarchyRoot.CreateChildComponent<UIBase>(UIManager.PrefabUIBase);
            _actionUI.Character = this;

            CombatPosition = TryGetComponent<SnapTo>(out var snapTo) ? snapTo.TargetPosition : transform.position;
            UpdateColors();
        }
        public override void PlayStart()
        {
            base.PlayStart();
            Equip(_StartingTool);
        }
        public override void PlayUpdate()
        {
            base.PlayUpdate();
            UpdateProgresses(Time.deltaTime);
            UpdateWalkAnimation(Time.deltaTime);
            InvokeEvents();

            _previousActionProgress = ActionProgress;
            _previousFocusProgress = FocusProgress;
            _previousWoundsCount = WoundsCount;
        }
        public override void PlayDestroy()
        {
            base.PlayDestroy();
            if (_actionUI != null)
                _actionUI.DestroyObject();
        }

#if UNITY_EDITOR
        public override void EditAwake()
        {
            base.EditAwake();
            if (GetComponent<TeamMember>()._StartingTeam.TryNonNull(out var team))
                GetComponent<SpriteRenderer>().color = team._Color;
            else
                GetComponent<SpriteRenderer>().color = Color.white;
        }
#endif
    }
}