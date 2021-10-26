namespace Vheos.Games.ActionPoints
{
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;

    public class UIActionPoint : AUIPoint
    {
        // Publics
        public void UpdateLocalProgresses(int index, float visualActionProgress, float visualFocusProgress)
        {
            ActionProgress = visualActionProgress.Abs().Sub(index).Clamp01();
            ActionColor = visualActionProgress >= 0 ? UIManager.Settings._PointActionColor : UIManager.Settings._PointExhaustColor;
            FocusProgress = visualFocusProgress.Sub(index).Clamp01();
        }

        // Privates
        private UIWound _uiWound;
        private void UpdateOpacityOnPointsCountChange(int previousCount, int currentCount)
        {
            previousCount = previousCount.Abs();
            currentCount = currentCount.Abs();
            if (Index >= previousCount && Index < currentCount)
                this.Animate(nameof(Opacity), v => Opacity = v, Opacity, 1f, UIManager.Settings._PointAnimDuration);
            if (Index >= currentCount && Index < previousCount)
                this.Animate(nameof(Opacity), v => Opacity = v, Opacity, UIManager.Settings._PointPartialProgressOpacity, UIManager.Settings._PointAnimDuration);
        }
        private void UpdateWoundVisibility(int previousCount, int currentCount)
        {
            int indexFromLast = UI.Character.RawMaxActionPoints - Index - 1;
            if (indexFromLast >= previousCount && indexFromLast < currentCount)
                _uiWound.Show(Index);
            if (indexFromLast >= currentCount && indexFromLast < previousCount)
                _uiWound.Hide();
        }

        // Play
        public override void PlayAwake()
        {
            base.PlayAwake();
            name = GetType().Name;

            Opacity = UIManager.Settings._PointPartialProgressOpacity;
            UI.Character.OnActionPointsCountChanged += UpdateOpacityOnPointsCountChange;

            _uiWound = this.CreateChildComponent<UIWound>(UIManager.Prefabs.Wound);
            UI.Character.OnWoundsCountChanged += UpdateWoundVisibility;
        }
    }
}