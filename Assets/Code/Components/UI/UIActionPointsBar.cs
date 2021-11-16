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

            if (TryGetComponent<MoveTowards>(out var moveTowards))
                moveTowards.Target = Character.transform;
            if (TryGetComponent<RotateAs>(out var rotateAs))
                rotateAs.Target = CameraManager.FirstActive.transform;

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
            transform.AnimateLocalScale(this, _originalScale, Settings.AnimDuration);
        }
        public void Hide(bool instantly = false)
        => transform.AnimateLocalScale(this, Vector3.zero, instantly ? 0f : Settings.AnimDuration, this.GODeactivate);
        public void NotifyExhausted()
        {
            for (int i = 0; i <= Character.Get<Actionable>().ActionPointsCount.Abs(); i++)
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
        protected override void AutoSubscribeToEvents()
        {
            base.AutoSubscribeToEvents();
            SubscribeTo(Get<Updatable>().OnUpdated, UpdateVisualProgresses);
        }
    }
}