namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.Math;
    using Tools.UnityCore;

    public class UIActionPointsBar : AUIPointsBar<UIActionPoint>
    {
        // Publics  
        public void Show()
        {
            this.GOActivate();
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
        protected override void PlayStart()
        {
            base.PlayStart();
            if (TryGetComponent<MoveTowards>(out var moveTowards))
                moveTowards.Target = Character.transform;
            if (TryGetComponent<RotateAs>(out var rotateAs))
                rotateAs.Target = CameraManager.FirstActive.transform;

            _originalScale = transform.localScale;
            CreatePoints(Character.Get<Actionable>().MaxActionPoints, UIManager.Settings.Prefab.ActionPoint);
            AlignPoints();
            Hide(true);
        }
        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            SubscribeTo(Get<Updatable>().OnUpdated, UpdateVisualProgresses);
        }
    }
}