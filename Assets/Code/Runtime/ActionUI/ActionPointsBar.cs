namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.Utilities;

    [RequireComponent(typeof(Expandable))]
    [RequireComponent(typeof(Updatable))]
    [DisallowMultipleComponent]
    public class ActionPointsBar : ABaseComponent
    {
        // Inspector
        [SerializeField] [Range(-1f, 1f)] protected float _Spacing;
        [SerializeField] [Range(0f, 1f)] protected float _HalfTime;

        // Events
        public AutoEvent<float, float> OnChangeVisualActionProgress = new();
        public AutoEvent<float, float> OnChangeVisualFocusProgress = new();

        // Publics
        public ActionUI UI
        { get; private set; }
        public IReadOnlyCollection<ActionPoint> Points
        => _points;

        // Privates
        private readonly HashSet<ActionPoint> _points = new();
        private float _visualActionProgess;
        private float _visualFocusProgess;
        private void CreatePoints()
        {
            int maxActionPoints = UI.Actionable.MaxActionPoints;
            ActionPoint pointPrefab = Settings.Prefabs.ActionPoint;
            Debug.Log($"{pointPrefab != null} / {maxActionPoints}");
            for (int i = 0; i < maxActionPoints; i++)
            {
                var newActionPoint = this.CreateChildComponent(pointPrefab);
                float offsetX = (i - maxActionPoints.Sub(1).Div(2)) * _Spacing.Add(1) * pointPrefab.transform.localScale.x;
                float offsetY = Vector2.down.y * pointPrefab.transform.localScale.y / 2f;

                newActionPoint.transform.localPosition = UI.Rect.Value.EdgePoint(Vector2.down).Add(offsetX, offsetY);
                newActionPoint.Initialize(this, i);
                _points.Add(newActionPoint);
            }
        }
        private void TryInvokeEvents()
        {
            float lerpAlpha = Utility.HalfTimeToLerpAlpha(_HalfTime);

            float previousVisualActionProgress = _visualActionProgess;
            _visualActionProgess.SetLerp(UI.Actionable.ActionProgress, lerpAlpha);
            if (_visualActionProgess != previousVisualActionProgress)
                OnChangeVisualActionProgress.Invoke(previousVisualActionProgress, _visualActionProgess);

            float previousVisualFocusProgress = _visualFocusProgess;
            _visualFocusProgess.SetLerp(UI.Actionable.FocusProgress, lerpAlpha);
            if (_visualFocusProgess != previousVisualFocusProgress)
                OnChangeVisualFocusProgress.Invoke(previousVisualFocusProgress, _visualFocusProgess);
        }
        private void UpdateInstantly()
        {
            OnChangeVisualActionProgress.Invoke(_visualActionProgess, UI.Actionable.ActionProgress);
            _visualActionProgess = UI.Actionable.ActionProgress;
            OnChangeVisualFocusProgress.Invoke(_visualFocusProgess, UI.Actionable.FocusProgress);
            _visualFocusProgess = UI.Actionable.FocusProgress;
        }

        // Play
        public void Initialize(ActionUI ui)
        {
            UI = ui;
            name = $"PointsBar";
            BindEnableDisable(ui);

            CreatePoints();

            Get<Updatable>().OnUpdate.SubEnableDisable(this, TryInvokeEvents);
            OnPlayEnable.Sub(UpdateInstantly);

            Get<Expandable>().OnStartExpanding.SubDestroy(this, () => IsActive = true);
            Get<Expandable>().OnFinishCollapsing.SubDestroy(this, () => IsActive = false);
            Get<Expandable>().ExpandTween.Set(() => this.NewTween().SetDuration(0.4f).LocalScale(Vector3.one));
            Get<Expandable>().CollapseTween.Set(() => this.NewTween().SetDuration(0.4f).LocalScale(Vector3.zero));
            Get<Expandable>().TryCollapse(true);
        }
    }
}