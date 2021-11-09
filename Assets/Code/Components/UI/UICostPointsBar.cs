namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.Extensions.Math;

    public class UICostPointsBar : AUIPointsBar<UICostPoint>
    {
        // Publics
        public UIButton Button
        { get; set; }
        public void NotifyUnfocused()
        {
            for (int i = 0; i < Button.Action.FocusPointsCost; i++)
                _points[_points.Count - 1 - i].PlayCantUseAnim();
        }

        // Overrides
        protected override IEnumerable<Vector2> GetAlignmentVectors()
        {
            switch (_points.Count)
            {
                case 1:
                    yield return Vector2.zero;
                    break;
                case 2:
                    yield return new Vector2(-0.5f, 0);
                    yield return new Vector2(+0.5f, 0);
                    break;
                case 3:
                    float triSide = Mathf.Sin(Mathf.PI / 3).Mul(2).Inv();
                    yield return NewUtility.PointOnCircle(270, triSide);
                    yield return NewUtility.PointOnCircle(150, triSide);
                    yield return NewUtility.PointOnCircle(30, triSide);
                    break;
                case 4:
                    yield return new Vector2(-0.5f, +0.5f);
                    yield return new Vector2(+0.5f, +0.5f);
                    yield return new Vector2(-0.5f, -0.5f);
                    yield return new Vector2(+0.5f, -0.5f);
                    break;
                case 5:
                    float pentaSide = Mathf.Sin(Mathf.PI / 5).Mul(2).Inv();
                    yield return NewUtility.PointOnCircle(270, pentaSide);
                    yield return NewUtility.PointOnCircle(198, pentaSide);
                    yield return NewUtility.PointOnCircle(126, pentaSide);
                    yield return NewUtility.PointOnCircle(54, pentaSide);
                    yield return NewUtility.PointOnCircle(-18, pentaSide);
                    break;
            }
        }

        // Play
        protected override void SubscribeToEvents()
        { }
        protected override void PlayStart()
        {
            base.PlayStart();
            transform.localPosition = Button.transform.localScale.Mul(+0.5f, -1f, 0f);
            int pointsCount = Button.Action.ActionPointsCost + Button.Action.FocusPointsCost;
            CreatePoints(pointsCount, UIManager.Settings.Prefab.CostPoint);
            AlignPoints();

            foreach (var point in _points)
                point.CostPointsBar = this;
        }
    }
}