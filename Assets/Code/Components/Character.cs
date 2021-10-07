namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.Extensions.Math;

    public class Character : AMousableSprite
    {
        // Inspector
        public GameObject _ActionUIPrefab = null;
        public Color _Color = Color.white;
        public List<Action> _Actions = new List<Action>();
        public int _MaxPoints = 5;
        public float _ActionSpeed = 1f;
        public float _FocusSpeed = 0.5f;
        public float _ExhaustSpeed = 0.5f;

        // Publics
        public delegate void PointsCountChanged(int actionPointsCount, int foucsPointsCount);
        public PointsCountChanged OnPointsCountChanged;
        public float ActionProgress
        { get; private set; }
        public float FocusProgress
        { get; private set; }
        public int ActionPointsCount
        => ActionProgress.RoundTowardsZero();
        public int FocusPointsCount
        => FocusProgress.RoundDown();

        // Private
        private ActionUI _actionUI;
        private float _previousActionProgress;
        private float _previousFocusProgress;
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
            int previousFocusPointsCount = _previousFocusProgress.RoundTowardsZero();
            int currentActionPointsCount = ActionProgress.RoundTowardsZero();
            int currentFocusPointsCount = FocusProgress.RoundTowardsZero();

            if (previousActionsPointsCount != ActionPointsCount
            || previousFocusPointsCount != FocusPointsCount)
                OnPointsCountChanged?.Invoke(currentActionPointsCount, currentFocusPointsCount);

            _previousActionProgress = ActionProgress;
            _previousFocusProgress = FocusProgress;
        }

        // Mouse
        public override void MousePress(MouseManager.Button button, Vector3 location)
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
        }
        public override void PlayAwake()
        {
            base.PlayAwake();
            _actionUI = ActionUI.Create(_ActionUIPrefab, this);
            _spriteRenderer.color = _Color;
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