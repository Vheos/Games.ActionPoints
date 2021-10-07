namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.Extensions.UnityObjects;
    using Vheos.Tools.Extensions.Math;

    public class ActionPoint : ACustomDrawable
    {
        // Publics
        static public ActionPoint Create(GameObject prefab, ActionPointsBar pointsBar, int index)
        {
            ActionPoint newPoint = Instantiate(prefab).GetComponent<ActionPoint>();
            newPoint.name = nameof(ActionPoint);
            newPoint.BecomeChildOf(pointsBar);
            newPoint.PointsBar = pointsBar;
            newPoint._index = index;

            newPoint.Initialize();
            return newPoint;
        }
        public ActionPointsBar PointsBar
        { get; private set; }
        public void UpdateLocalProgresses(float visualActionProgress, float visualFocusProgress)
        {
            ActionProgress = visualActionProgress.Abs().Sub(_index).Clamp01();
            Opacity = ActionProgress >= 0.99f 
                    ? PointsBar._OpacityFull 
                    : PointsBar._OpacityFrom.Lerp(PointsBar._OpacityTo, ActionProgress);
            ActionColor = visualActionProgress >= 0 
                        ? PointsBar._ActionProgressColor
                        : PointsBar._ExhaustProgressColor;
            FocusProgress = visualFocusProgress.Sub(_index).Clamp01();
        }

        // Privates
        private int _index;
        private void Initialize()
        {
            BackgroundColor = PointsBar._BackgroundColor;
            ActionColor = PointsBar._ActionProgressColor;
            FocusColor = PointsBar._FocusProgressColor;
        }

        // MProps
        public Color BackgroundColor
        {
            get => GetColor(nameof(MProp.ColorC));
            set => SetColor(nameof(MProp.ColorC), value);
        }
        public Color ActionColor
        {
            get => GetColor(nameof(MProp.ColorB));
            set => SetColor(nameof(MProp.ColorB), value);
        }
        public Color FocusColor
        {
            get => GetColor(nameof(MProp.ColorA));
            set => SetColor(nameof(MProp.ColorA), value);
        }
        public float Opacity
        {
            get => GetFloat(nameof(MProp.Opacity));
            set => SetFloat(nameof(MProp.Opacity), value);
        }
        public float ActionProgress
        {
            get => GetFloat(nameof(MProp.ThresholdB));
            set => SetFloat(nameof(MProp.ThresholdB), value);
        }
        public float FocusProgress
        {
            get => GetFloat(nameof(MProp.ThresholdA));
            set => SetFloat(nameof(MProp.ThresholdA), value);
        }
        private enum MProp
        {
            ColorA,
            ColorB,
            ColorC,
            ThresholdA,
            ThresholdB,
            Opacity,
        }
    }
}