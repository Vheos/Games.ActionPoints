namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.Utilities;
    using System.Linq;
    using Vheos.Tools.Extensions.General;
    using Vheos.Tools.Extensions.Collections;

    [RequireComponent(typeof(Expandable))]
    [RequireComponent(typeof(Updatable))]
    [DisallowMultipleComponent]
    public class ActionPointsBar : AActionUIElementsGroup<ActionPoint>
    {
        // Inspector
        [field: SerializeField, Range(-1f, 1f)] public float Spacing { get; private set; }
        [field: SerializeField, Range(0f, 1f)] public float HalfTime { get; private set; }

        // Events
        public AutoEvent<float, float> OnChangeVisualActionProgress = new();
        public AutoEvent<float, float> OnChangeVisualFocusProgress = new();
        public AutoEvent<int, int> OnChangeRealActionPoints
        => UI.Actionable.OnChangeActionPoints;
        public AutoEvent<int, int> OnChangeRealFocusPoints
        => UI.Actionable.OnChangeFocusPoints;

        // Privates
        private float _visualActionProgress;
        private float _visualFocusProgress;
        private void UpdateVisualProgresses()
        {
            float lerpAlpha = Utility.HalfTimeToLerpAlpha(HalfTime);

            float previousVisualActionProgress = _visualActionProgress;
            _visualActionProgress = _visualActionProgress.Lerp(UI.Actionable.ActionProgress, lerpAlpha);
            if (_visualActionProgress != previousVisualActionProgress)
                OnChangeVisualActionProgress.Invoke(previousVisualActionProgress, _visualActionProgress);

            float previousVisualFocusProgress = _visualFocusProgress;
            _visualFocusProgress = _visualFocusProgress.Lerp(UI.Actionable.FocusProgress, lerpAlpha);
            if (_visualFocusProgress != previousVisualFocusProgress)
                OnChangeVisualFocusProgress.Invoke(previousVisualFocusProgress, _visualFocusProgress);
        }
        private void UpdatePointsInstantly()
        {
            _visualActionProgress = UI.Actionable.ActionProgress;
            _visualFocusProgress = UI.Actionable.FocusProgress;
            int realActionPoints = UI.Actionable.ActionPoints;
            int realFocusPoints = UI.Actionable.FocusPoints;
            foreach (var point in _elements)
                point.UpdateInstantly(_visualActionProgress, _visualFocusProgress, realActionPoints, realFocusPoints);
        }

        // Common
        private void UpdatePointsCount(int from, int to)
        {
            int deltaPoints = to - from;
            if (deltaPoints < 0)
                DestroyPoints(-deltaPoints);
            else if (deltaPoints > 0)
                CreatePoints(deltaPoints);

            UpdatePositionsAndIndices();
            UpdatePointsInstantly();

            _newElements.Clear();
        }
        private void DestroyPoints(int count)
        {
            for (int i = _elements.Count - count; i < _elements.Count; i++)
                _elements[i].AnimateDestroy(!isActiveAndEnabled);
            _elements.RemoveRange(_elements.Count - count, count);
        }
        private void CreatePoints(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var newPoint = this.CreateChildComponent(Settings.Prefabs.ActionPoint);
                newPoint.Initialize(this);
                newPoint.AnimateCreate(!isActiveAndEnabled);
                _newElements.Add(newPoint);
            }
            _elements.Add(_newElements);
        }
        private void UpdatePositionsAndIndices()
        {
            var prefabScale = Settings.Prefabs.ActionPoint.transform.localScale;
            float offsetY = Vector2.down.y * prefabScale.y / 2f;
            for (int i = 0; i < _elements.Count; i++)
            {
                _elements[i].SetIndex(i);

                float offsetX = (i - _elements.Count.Sub(1).Div(2)) * Spacing.Add(1) * prefabScale.x;
                Vector2 localPosition = UI.Rect.Value.EdgePoint(Vector2.down).Add(offsetX, offsetY);
                bool isNew = _newElements.Contains(_elements[i]);
                _elements[i].AnimateMove(localPosition, !isActiveAndEnabled || isNew);
            }
        }

        // Play
        override public void Initialize(ActionUI ui)
        {
            base.Initialize(ui);

            OnPlayEnable.Sub(UpdatePointsInstantly);

            UI.Actionable.OnChangeMaxActionPoints.SubDestroy(this, UpdatePointsCount);

            Get<Updatable>().OnUpdate.SubEnableDisable(this, UpdateVisualProgresses);
        }
    }
}