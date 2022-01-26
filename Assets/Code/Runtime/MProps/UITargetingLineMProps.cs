namespace Vheos.Games.ActionPoints
{
    sealed public class UITargetingLineMProps : AAutoMProps
    {
        // Publics
        public float TilingX
        {
            get => GetFloat(nameof(TilingX));
            set => SetFloat(nameof(TilingX), value);
        }
    }
}