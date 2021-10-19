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
        public int _MaxPoints = 5;
        public float _ActionSpeed = 1f;
        public float _FocusSpeed = 0.5f;
        public float _ExhaustSpeed = 0.5f;

        // Publics
        public delegate void PointsCountChanged(int previous, int current);
        public PointsCountChanged OnActionPointsCountChanged;
        public PointsCountChanged OnFocusPointsCountChanged;
        public float ActionProgress
        { get; private set; }
        public float FocusProgress
        { get; private set; }
        public int ActionPointsCount
        => ActionProgress.RoundTowardsZero();
        public int FocusPointsCount
        => FocusProgress.RoundDown();
        public bool IsExhausted
        => ActionProgress < 0;
        public void ChangeActionPoints(int diff)
        {
            ActionProgress += diff;
            ActionProgress.Clamp(-_MaxPoints, +_MaxPoints);
        }
        public void ChangeFocusPoints(int diff)
        {
            FocusProgress += diff;
            FocusProgress.Clamp(0, _MaxPoints);
        }
        public void GainTargeting(Action action)
        {
            _spriteOutline.Grow();
        }
        public void LoseTargeting()
        {
            _spriteOutline.Shrink();
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
        private float _previousActionProgress;
        private float _previousFocusProgress;
        private SpriteOutline _spriteOutline;
        private Animator _animator;
        private Vector3 _previousPosition;
        private void UpdateProgresses(float deltaTime)
        {
            ActionProgress += deltaTime * _ActionSpeed;
            if (ActionProgress > _MaxPoints)
            {
                deltaTime = (ActionProgress - _MaxPoints) / _ActionSpeed;
                ActionProgress = _MaxPoints;
                FocusProgress += deltaTime * _FocusSpeed;
                FocusProgress = FocusProgress.ClampMax(ActionProgress);
            }
        }
        private void InvokePointsCountChangedEvents()
        {
            int previousActionsPointsCount = _previousActionProgress.RoundTowardsZero();
            int currentActionPointsCount = ActionProgress.RoundTowardsZero();
            if (previousActionsPointsCount != ActionPointsCount)
                OnActionPointsCountChanged?.Invoke(previousActionsPointsCount, currentActionPointsCount);
            else if (_previousActionProgress < 0 && ActionProgress >= 0)
                OnActionPointsCountChanged(0, 0);

            int previousFocusPointsCount = _previousFocusProgress.RoundTowardsZero();
            int currentFocusPointsCount = FocusProgress.RoundTowardsZero();
            if (previousFocusPointsCount != FocusPointsCount)
                OnFocusPointsCountChanged?.Invoke(previousFocusPointsCount, currentFocusPointsCount);

            _previousActionProgress = ActionProgress;
            _previousFocusProgress = FocusProgress;
        }

        // Mouse
        public override void MousePress(CursorManager.Button button, Vector3 location)
        {
            base.MousePress(button, location);
            _actionUI.ToggleWheel();
        }
        public override void MouseGainHighlight()
        {
            base.MouseGainHighlight();
            _actionUI.CollapseOtherWheels();
            _actionUI.ExpandWheel();
        }

        // Mono
        public override void PlayUpdate()
        {
            base.PlayUpdate();
            UpdateProgresses(Time.deltaTime);
            InvokePointsCountChangedEvents();

            float speed = _previousPosition.DistanceTo(transform.position) / Time.deltaTime;
            _animator.SetFloat("Speed", speed);

            _previousPosition = transform.position;
        }
        public override void PlayAwake()
        {
            base.PlayAwake();
            _animator = GetComponent<Animator>();
            _spriteOutline = GetComponent<SpriteOutline>();
            _spriteRenderer.color = _Color;

            _actionUI = UIManager.HierarchyRoot.CreateChild<UIBase>(UIManager.PrefabUIBase);
            _actionUI.Character = this;
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