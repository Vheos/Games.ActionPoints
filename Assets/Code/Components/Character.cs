namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.Extensions.Math;
    [RequireComponent(typeof(SpriteOutline))]
    public class Character : AMousableSprite
    {
        // Constants
        const string UI_ROOT_NAME = "UI";

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
        public void GainTargeting(Action action)
        {
            _spriteOutline.Grow();
        }
        public void LoseTargeting()
        {
            _spriteOutline.Shrink();
        }
        public void ChangeActionPoints(int diff)
        => ActionProgress += diff;
        public void ChangeFocusPoints(int diff)
        => FocusProgress += diff;

        // Private
        static private Transform _uiRoot;
        private UIBase _actionUI;
        private float _previousActionProgress;
        private float _previousFocusProgress;
        private SpriteOutline _spriteOutline;
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
        }
        public override void PlayAwake()
        {
            base.PlayAwake();
            _spriteOutline = GetComponent<SpriteOutline>();
            _spriteRenderer.color = _Color;

            if (_uiRoot == null)
                _uiRoot = new GameObject(UI_ROOT_NAME).transform;
            _actionUI = _uiRoot.CreateChild<UIBase>(_ActionUIPrefab);
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