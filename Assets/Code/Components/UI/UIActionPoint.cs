namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.Extensions.UnityObjects;
    using Vheos.Tools.Extensions.Math;

    public class UIActionPoint : AUIPoint
    {
        // Inspector
        public QAnimFloat _ActionOpacityAnim = new QAnimFloat();

        // Publics
        public void UpdateLocalProgresses(int index, float visualActionProgress, float visualFocusProgress)
        {
            ActionProgress = visualActionProgress.Abs().Sub(index).Clamp01();
            ActionColor = visualActionProgress >= 0 ? UI._PointActionColor : UI._PointExhaustColor;
            FocusProgress = visualFocusProgress.Sub(index).Clamp01();
        }

        // Privates
        private void UpdateOpacityOnPointsCountChange(int previousCount, int currentCount)
        {
            previousCount = previousCount.Abs();
            currentCount = currentCount.Abs();
            if (Index >= previousCount && Index <= currentCount - 1)
                _ActionOpacityAnim.Start(Opacity, 1f);
            if (Index >= currentCount && Index <= previousCount - 1)
                _ActionOpacityAnim.Start(Opacity, UI._PointPartialProgressOpacity);
        }

        // Mono
        public override void PlayStart()
        {
            base.PlayStart();
            name = GetType().Name;

            Opacity = UI._PointPartialProgressOpacity;
            UI.Character.OnActionPointsCountChanged += UpdateOpacityOnPointsCountChange;
        }
        public override void PlayUpdate()
        {
            base.PlayUpdate();
            if (_ActionOpacityAnim.IsActive)
                Opacity = _ActionOpacityAnim.Value;
        }
    }
}