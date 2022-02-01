namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using Games.Core;
    using Tools.Extensions.Math;
    using System.Collections.Generic;
    using Tools.Extensions.UnityObjects;
    using Vheos.Tools.Utilities;

    [RequireComponent(typeof(Actionable))]
    [DisallowMultipleComponent]
    public class ActionPointsBar : ABaseComponent
    {
        // Inspector
        [SerializeField] protected Transform _PointsBarPrefab;
        [SerializeField] protected ActionPoint _PointPrefab;
        [SerializeField] [Range(-1f, 1f)] protected float _Spacing;
        [SerializeField] [Range(0f, 1f)] protected float _HalfTime;

        // Events
        public AutoEvent<float, float> OnChangeVisualActionProgress = new();
        public AutoEvent<float, float> OnChangeVisualFocusProgress = new();

        // Publics

        // Privates
        private Transform _actionPointBar;
        private HashSet<ActionPoint> _actionPoints;
        private float _visualActionProgess;
        private float _visualFocusProgess;
        private void TryInvokeEvents()
        {
            float lerpAlpha = Utility.HalfTimeToLerpAlpha(_HalfTime);

            float previousVisualActionProgress = _visualActionProgess;
            _visualActionProgess.SetLerp(Get<Actionable>().ActionProgress, lerpAlpha);
            if (_visualActionProgess != previousVisualActionProgress)
                OnChangeVisualActionProgress.Invoke(previousVisualActionProgress, _visualActionProgess);

            float previousVisualFocusProgress = _visualFocusProgess;
            _visualFocusProgess.SetLerp(Get<Actionable>().FocusProgress, lerpAlpha);
            if (_visualFocusProgess != previousVisualFocusProgress)
                OnChangeVisualFocusProgress.Invoke(previousVisualFocusProgress, _visualFocusProgess);
        }

        // Play
        protected override void PlayStart()
        {
            base.PlayStart();

            _actionPointBar = Instantiate(_PointsBarPrefab);
            _actionPointBar.name = $"{typeof(ActionPointsBar).Name}";
            _actionPointBar.GetComponent<MoveTowards>().SetTarget(this, true);
            _actionPointBar.GetComponent<RotateAs>().SetTarget(CameraManager.AnyNonUI, true);

            _actionPoints = new();
            int maxActionPoints = Get<Actionable>().MaxActionPoints;
            for (int i = 0; i < maxActionPoints; i++)
            {
                var newActionPoint = _actionPointBar.CreateChildComponent(_PointPrefab, $"ActionPoint{i + 1}");
                newActionPoint.transform.SetLocalPositionX((i - maxActionPoints.Sub(1).Div(2f)) * (1 + _Spacing));
                newActionPoint.Initialize(this, i);
                _actionPoints.Add(newActionPoint);
            }

            Get<Updatable>().OnUpdate.SubscribeAuto(this, TryInvokeEvents);
        }
    }
}