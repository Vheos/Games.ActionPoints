namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;

    public class UICostPointsBar : AUIPointsBar<AUIPoint>
    {
        // Publics
        public UIButton Button
        { get; set; }

        // Mono
        public override void PlayStart()
        {
            base.PlayStart();
            transform.localPosition = Button.transform.localScale.Mul(+0.5f, -1f, 0f);
            int pointsCount = Button.Action.ActionPointsCost + Button.Action.FocusPointsCost;
            CreatePoints(pointsCount, UI._PrefabCostPoint);
            AlignPoints();
        }
    }
}