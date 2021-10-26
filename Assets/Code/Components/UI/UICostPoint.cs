namespace Vheos.Games.ActionPoints
{
    public class UICostPoint : AUIPoint
    {
        // Publics
        public UICostPointsBar CostPointsBar
        { get; set; }

        // Play
        public override void PlayStart()
        {
            base.PlayStart();
            Opacity = Settings.CostOpacity;
            ActionProgress = 1f;

            bool isActionPoint = Index < CostPointsBar.Button.Action.ActionPointsCost;
            FocusProgress = isActionPoint ? 0f : 1f;
            Shape = isActionPoint ? Settings.ActionShape : Settings.FocusShape;
        }
    }
}