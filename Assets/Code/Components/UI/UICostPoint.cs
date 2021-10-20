namespace Vheos.Games.ActionPoints
{
    using UnityEngine;

    public class UICostPoint : AUIPoint
    {
        // Inspector
        [Range(0f, 1f)] public float _Opacity = 0.5f;

        // Publics
        public UICostPointsBar CostPointsBar
        { get; set; }

        // Mono
        public override void PlayStart()
        {
            base.PlayStart();
            Opacity = _Opacity;
            ActionProgress = 1f;

            bool isActionPoint = Index < CostPointsBar.Button.Action._ActionPointsCost;
            FocusProgress = isActionPoint ? 0f : 1f;
            Shape = isActionPoint ? UI._PointActionShape : UI._PointFocusShape;
        }
    }
}

/*
// Privates
public void UpdateColor(int _, int actionPointsCount)
=> ActionColor = Index < actionPointsCount ? UI._PointActionColor : UI._PointExhaustColor;

UpdateColor(0, UI.Character.ActionPointsCount);
UI.Character.OnActionPointsCountChanged += UpdateColor;
*/