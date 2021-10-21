namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;

    [RequireComponent(typeof(SpriteOutline))]
    [RequireComponent(typeof(Animator))]
    public class Character : AMousableSprite
    {
        // Inspector
        public Color _Color = Color.white;
        public List<Action> _Actions = new List<Action>();
        public int _RawMaxPoints = 5;
        public float _ActionSpeed = 1f;
        public float _FocusSpeed = 0.5f;
        public float _ExhaustSpeed = 0.5f;
        public float _BluntArmor = 0f;
        public float _SharpArmor = 0f;

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
        public void GainTargeting(Action action)
        {
            _spriteOutline.Show();
        }
        public void LoseTargeting()
        {
            _spriteOutline.Hide();
        }
        public void LookAt(Transform targetTransform)
        {
            if (targetTransform == transform)
                return;

            if (TryGetComponent<RotateTowards>(out var rotateTowards))
                rotateTowards._Target = targetTransform;
            else
                transform.rotation.SetLookRotation(transform.DirectionTowards(targetTransform));
        }

        // Private
        private UIBase _actionUI;
        private SpriteOutline _spriteOutline;
        private Animator _animator;
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

            _previousActionProgress = ActionProgress;
            _previousFocusProgress = FocusProgress;
            _previousWoundsCount = WoundsCount;
        }
        public delegate void PointsCountChanged(int previous, int current);
        public delegate void ExhaustStateChangedZero(bool isExhausted);
        public delegate void WoundsCountChanged(int previous, int current);
        public PointsCountChanged OnActionPointsCountChanged;
        public PointsCountChanged OnFocusPointsCountChanged;
        public ExhaustStateChangedZero OnExhaustStateChanged;
        public WoundsCountChanged OnWoundsCountChanged;

        // Mouse
        public override void MousePress(CursorManager.Button button, Vector3 location)
        {
            base.MousePress(button, location);
            _actionUI.ToggleWheel();
        }

        // Mono
        public override void PlayUpdate()
        {
            base.PlayUpdate();
            UpdateProgresses(Time.deltaTime);
            InvokeEvents();
            UpdateWalkAnimation(Time.deltaTime);
        }
        public override void PlayAwake()
        {
            base.PlayAwake();
            _animator = GetComponent<Animator>();
            _spriteOutline = GetComponent<SpriteOutline>();
            _spriteRenderer.color = _Color;

            _actionUI = UIManager.HierarchyRoot.CreateChildComponent<UIBase>(UIManager.PrefabUIBase);
            _actionUI.Character = this;
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
            GetComponent<SpriteRenderer>().color = _Color;
        }
#endif
    }
}