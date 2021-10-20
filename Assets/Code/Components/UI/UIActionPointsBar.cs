namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;

    public class UIActionPointsBar : AUIPointsBar<UIActionPoint>
    {
        // Privates
        private float _visualActionProgress;
        private float _visualFocusProgress;
        private void UpdateVisualProgresses()
        {
            float lerpAlpha = NewUtility.LerpHalfTimeToAlpha(UI._PointVisualProgressHalfTime);
            _visualActionProgress = _visualActionProgress.Lerp(UI.Character.ActionProgress, lerpAlpha);
            _visualFocusProgress = _visualFocusProgress.Lerp(UI.Character.FocusProgress, lerpAlpha);
        }
        private void UpdatePoints()
        {
            for (int i = 0; i < _points.Count; i++)
                _points[i].UpdateLocalProgresses(i, _visualActionProgress, _visualFocusProgress);
        }

        // Publics
        public void NotifyExhausted()
        {
            for (int i = 0; i <= UI.Character.ActionPointsCount.Abs(); i++)
                _points[i].PlayCantUseAnim();
        }

        // Mono
        public override void PlayStart()
        {
            base.PlayStart();

            if (TryGetComponent<MoveTowards>(out var moveTowards))
                moveTowards._Target = UI.Character.transform;
            if (TryGetComponent<RotateAs>(out var rotateAs))
                rotateAs._Target = CameraManager.FirstActive.transform;

            CreatePoints(UI.Character._RawMaxPoints, UI._PrefabActionPoint);
            AlignPoints();
        }
        override public void PlayUpdate()
        {
            base.PlayUpdate();
            UpdateVisualProgresses();
            UpdatePoints();
        }
    }
}