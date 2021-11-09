namespace Vheos.Games.ActionPoints
{
    public class UICostPoint : AUIPoint
    {
        // Publics
        public UICostPointsBar CostPointsBar
        { get; set; }

        // Play
        protected override void PlayStart()
        {
            base.PlayStart();
            _drawable.Opacity = Settings.CostOpacity;
            _drawable.ActionProgress = 1f;

            bool isActionPoint = Index < CostPointsBar.Button.Action.ActionPointsCost;
            _drawable.FocusProgress = isActionPoint ? 0f : 1f;
            _drawable.Shape = isActionPoint ? Settings.ActionShape : Settings.FocusShape;
        }

        protected override void SubscribeToEvents()
        { }
    }
}