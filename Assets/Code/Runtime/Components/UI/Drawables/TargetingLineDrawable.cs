namespace Vheos.Games.ActionPoints
{
    sealed public class TargetingLineDrawable : ACustomDrawable
    {
        // Publics
        public float TilingX
        {
            get => GetFloat(nameof(TilingX));
            set => SetFloat(nameof(TilingX), value);
        }
    }
}