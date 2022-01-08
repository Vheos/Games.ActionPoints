namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.Math;
    using Tools.UnityCore;

    public class UIActionPointsBar : AUIPointsBar<UIActionPoint>
    {
        // Publics  
        public void Initialize()
        {
            CreatePoints(Character.Get<Actionable>().MaxActionPoints, UIManager.Settings.Prefab.ActionPoint);
            foreach (var point in _points)
                point.Initialize();

            if (TryGet<MoveTowards>(out var moveTowards))
                moveTowards.SetTarget(Character);
            if (TryGet<RotateAs>(out var rotateAs))
                rotateAs.SetTarget(CameraManager.FirstActive);

            _originalScale = transform.localScale;
            AlignPoints();
            Hide(true);
        }
        public void Show()
        {
            this.GOActivate();
            _visualActionProgress = 0;
            _visualFocusProgress = 0;
            foreach (var point in _points)
                point.ResetVisuals();
            this.NewTween()
                .SetDuration(Settings.AnimDuration)
                .SetConflictResolution(ConflictResolution.Interrupt)
                .LocalScale(_originalScale);
        }
        public void Hide(bool isInstant = false)
        => this.NewTween()
                .SetDuration(isInstant ? 0f : Settings.AnimDuration)
                .SetConflictResolution(ConflictResolution.Interrupt)
               .LocalScale(Vector3.zero)
               .AddOnFinishEvents(this.GODeactivate);
        public void NotifyExhausted()
        {
            for (int i = 0; i < Character.Get<Actionable>().UsableMaxActionPoints; i++)
                _points[i].PlayCantUseAnim();
        }

        // Privates
        private Vector3 _originalScale;
        private float _visualActionProgress;
        private float _visualFocusProgress;
        private void UpdateVisualProgresses()
        {
            float lerpAlpha = NewUtility.LerpHalfTimeToAlpha(Settings.VisualProgressHalfTime);
            _visualActionProgress = _visualActionProgress.Lerp(Character.Get<Actionable>().ActionProgress, lerpAlpha);
            _visualFocusProgress = _visualFocusProgress.Lerp(Character.Get<Actionable>().FocusProgress, lerpAlpha);

            for (int i = 0; i < _points.Count; i++)
                _points[i].UpdateLocalProgresses(_visualActionProgress, _visualFocusProgress);
        }

        // Play
        protected override void DefineAutoSubscriptions()
        {
            base.DefineAutoSubscriptions();
            SubscribeTo(Get<Updatable>().OnUpdate, UpdateVisualProgresses);
        }
    }
}