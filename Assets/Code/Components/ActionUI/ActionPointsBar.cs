namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;

    public class ActionPointsBar : AUpdatable
    {
        // Inspector
        public Color _BackgroundColor = Color.black;
        public Color _ActionProgressColor = Color.white;
        public Color _ExhaustProgressColor = Color.red;
        public Color _FocusProgressColor = Color.cyan;
        [Range(0f, 1f)] public float _OpacityFrom = 1 / 3f;
        [Range(0f, 1f)] public float _OpacityTo = 2 / 3f;
        [Range(0f, 1f)] public float _OpacityFull = 1f;
        [Range(0f, 1f)] public float _HalfTime = 0.2f;
        [Range(-0.5f, +0.5f)] public float _PointsSpacing = 0;

        // Publics
        static public ActionPointsBar Create(GameObject prefab, ActionUI ui)
        {
            ActionPointsBar newPointsBar = Instantiate(prefab).GetComponent<ActionPointsBar>();
            newPointsBar.name = nameof(ActionPointsBar);
            newPointsBar.BecomeChildOf(ui);
            newPointsBar.UI = ui;

            newPointsBar.Initialize();
            return newPointsBar;
        }
        public ActionUI UI
        { get; private set; }

        // Privates
        private List<ActionPoint> _actionPoints;
        private float _visualActionProgress;
        private float _visualFocusProgress;
        private void Initialize()
        {
            _actionPoints = new List<ActionPoint>();
            CreatePoints();

            if (TryGetComponent<MoveTowards>(out var moveTowards))
                moveTowards._Target = UI.Character.transform;
            if (TryGetComponent<RotateAs>(out var rotateAs))
                rotateAs._Target = CameraManager.FirstActive.transform;
        }

        // Methods
        private void CreatePoints()
        {
            int maxPoints = UI.Character._MaxPoints;
            for (int i = 0; i < maxPoints; i++)
            {
                ActionPoint newPoint = ActionPoint.Create(UI._PointPrefab, this, i);
                float localX = (i - maxPoints.Sub(1) / 2f) * (1 + _PointsSpacing) * newPoint.transform.localScale.x;
                newPoint.transform.SetLocalX(localX);
                _actionPoints.Add(newPoint);

                // 
            }
        }
        private void UpdateVisualProgresses()
        {
            float lerpAlpha = NewUtility.LerpHalfTimeToAlpha(_HalfTime);
            _visualActionProgress = _visualActionProgress.Lerp(UI.Character.ActionProgress, lerpAlpha);
            _visualFocusProgress = _visualFocusProgress.Lerp(UI.Character.FocusProgress, lerpAlpha);
        }
        private void UpdatePoints()
        {
            foreach (var point in _actionPoints)
                point.UpdateLocalProgresses(_visualActionProgress, _visualFocusProgress);
        }

        override public void PlayUpdate()
        {
            base.PlayUpdate();
            UpdateVisualProgresses();
            UpdatePoints();
        }
    }
}