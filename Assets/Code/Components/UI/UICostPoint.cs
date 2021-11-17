namespace Vheos.Games.ActionPoints
{
    public class UICostPoint : AUIPoint
    {
        // Publics
        public UICostPointsBar CostPointsBar
        { get; set; }
        public void Initialize(UICostPointsBar costPointsBar)
        {
            CostPointsBar = costPointsBar;

            _drawable.Opacity = Settings.CostOpacity;
            _drawable.ActionProgress = 1f;

            bool isActionPoint = Index < CostPointsBar.Button.Action.ActionPointsCost;
            _drawable.FocusProgress = isActionPoint ? 0f : 1f;
            _drawable.Shape = isActionPoint ? Settings.ActionShape : Settings.FocusShape;
        }
    }
}