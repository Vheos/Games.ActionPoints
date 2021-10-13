namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.Extensions.UnityObjects;
    using Vheos.Tools.Extensions.Math;

    public class UIActionPoint : AUIPoint
    {
        // Constants
        const float ANIMATION_THRESHOLD = 0.99f;

        // Inspector
        public QAnimFloat _ActionOpacityAnim = new QAnimFloat();

        // Publics
        public void UpdateLocalProgresses(int index, float visualActionProgress, float visualFocusProgress)
        {
            ActionProgress = visualActionProgress.Abs().Sub(index).Clamp01();
            ActionColor = visualActionProgress >= 0 ? UI._PointActionColor : UI._PointExhaustColor;
            FocusProgress = visualFocusProgress.Sub(index).Clamp01();

            if (_previousActionProgress < ANIMATION_THRESHOLD
            && ActionProgress >= ANIMATION_THRESHOLD)
                _ActionOpacityAnim.Start(Opacity, 1f);
            else if (_previousActionProgress >= ANIMATION_THRESHOLD
            && ActionProgress < ANIMATION_THRESHOLD)
                _ActionOpacityAnim.Start(Opacity, UI._PointPartialProgressOpacity);

            _previousActionProgress = ActionProgress;
        }

        // Privates
        private float _previousActionProgress;

        // Mono
        public override void PlayStart()
        {
            base.PlayStart();
            name = GetType().Name;

            Opacity = UI._PointPartialProgressOpacity;
        }
        public override void PlayUpdate()
        {
            base.PlayUpdate();
            if (_ActionOpacityAnim.IsActive)
                Opacity = _ActionOpacityAnim.Value;
        }
    }
}