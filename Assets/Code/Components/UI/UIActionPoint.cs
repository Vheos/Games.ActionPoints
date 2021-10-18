namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.Extensions.UnityObjects;
    using Vheos.Tools.Extensions.Math;

    public class UIActionPoint : AUIPoint
    {       
        // Inspector
        [Range(0f, 1f)] public float _AnimDuration;

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
                this.Animate(nameof(Opacity), v => Opacity = v, Opacity, 1f, _AnimDuration);
            if (Index >= currentCount && Index <= previousCount - 1)
                this.Animate(nameof(Opacity), v => Opacity = v, Opacity, UI._PointPartialProgressOpacity, _AnimDuration);
        }

        // Mono
        public override void PlayStart()
        {
            base.PlayStart();
            name = GetType().Name;

            Opacity = UI._PointPartialProgressOpacity;
            UI.Character.OnActionPointsCountChanged += UpdateOpacityOnPointsCountChange;
        }
    }
}