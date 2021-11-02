namespace Vheos.Games.ActionPoints
{
    using Tools.Extensions.Math;
    using UnityEngine;

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
            for (int i = 0; i <= UI.Character.ActionPointsCount.Abs(); i++)
                _points[i].PlayCantUseAnim();
        }

        // Privates
        private Vector3 _originalScale;
        private float _visualActionProgress;
        private float _visualFocusProgress;
        private void UpdateVisualProgresses()
        {
            float lerpAlpha = NewUtility.LerpHalfTimeToAlpha(Settings.VisualProgressHalfTime);
            _visualActionProgress = _visualActionProgress.Lerp(UI.Character.ActionProgress, lerpAlpha);
            _visualFocusProgress = _visualFocusProgress.Lerp(UI.Character.FocusProgress, lerpAlpha);
        }
        private void UpdatePoints()
        {
            for (int i = 0; i < _points.Count; i++)
                _points[i].UpdateLocalProgresses(_visualActionProgress, _visualFocusProgress);
        }

        // Play
        public override void PlayStart()
        {
            base.PlayStart();
            if (TryGetComponent<MoveTowards>(out var moveTowards))
                moveTowards.Target = UI.Character.transform;
            if (TryGetComponent<RotateAs>(out var rotateAs))
                rotateAs.Target = CameraManager.FirstActive.transform;

            _originalScale = transform.localScale;
            CreatePoints(UI.Character.RawMaxActionPoints, UIManager.Settings.Prefab.ActionPoint);
            AlignPoints();
            Hide(true);
        }
        protected override void SubscribeToPlayEvents()
        {
            base.SubscribeToPlayEvents();
            Updatable.OnPlayUpdate += () =>
            {
                UpdateVisualProgresses();
                UpdatePoints();
            };
        }
    }
}