namespace Vheos.Games.ActionPoints
{
    using Tools.Extensions.Math;

    public class UIActionPointsBar : AUIPointsBar<UIActionPoint>
    {
        // Publics
        public void NotifyExhausted()
        {
            for (int i = 0; i <= UI.Character.ActionPointsCount.Abs(); i++)
                _points[i].PlayCantUseAnim();
        }

        // Privates
        private float _visualActionProgress;
        private float _visualFocusProgress;
        private void UpdateVisualProgresses()
        {
            float lerpAlpha = NewUtility.LerpHalfTimeToAlpha(UIManager.Settings._PointVisualProgressHalfTime);
            _visualActionProgress = _visualActionProgress.Lerp(UI.Character.ActionProgress, lerpAlpha);
            _visualFocusProgress = _visualFocusProgress.Lerp(UI.Character.FocusProgress, lerpAlpha);
        }
        private void UpdatePoints()
        {
            for (int i = 0; i < _points.Count; i++)
                _points[i].UpdateLocalProgresses(i, _visualActionProgress, _visualFocusProgress);
        }

        // Play
        public override void PlayStart()
        {
            base.PlayStart();
            if (TryGetComponent<MoveTowards>(out var moveTowards))
                moveTowards.Target = UI.Character.transform;
            if (TryGetComponent<RotateAs>(out var rotateAs))
                rotateAs.Target = CameraManager.FirstActive.transform;

            CreatePoints(UI.Character.RawMaxActionPoints, UIManager.Prefabs.ActionPoint);
            AlignPoints();
        }
        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            OnPlayUpdate += () =>
            {
                UpdateVisualProgresses();
                UpdatePoints();
            };
        }
    }
}