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
            Opacity = UIManager.Settings._Opacity;
            ActionProgress = 1f;

            bool isActionPoint = Index < CostPointsBar.Button.Action.ActionPointsCost;
            FocusProgress = isActionPoint ? 0f : 1f;
            Shape = isActionPoint ? UIManager.Settings._PointActionShape : UIManager.Settings._PointFocusShape;
        }
    }
}