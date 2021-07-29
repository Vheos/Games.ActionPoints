namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    public class ActionPointMatProps : AMatProps
    {
        // Publics
        public Color BackgroundTint
        {
            get => MPBlock.GetColor("BackgroundTint");
            set => MPBlock.SetColor("BackgroundTint", value);
        }
        public Color ProgressTint
        {
            get => MPBlock.GetColor("ProgressTint");
            set => MPBlock.SetColor("ProgressTint", value);
        }
        public float Opacity
        {
            get => MPBlock.GetFloat("Opacity");
            set => MPBlock.SetFloat("Opacity", value);
        }
        public float Progress
        {
            get => MPBlock.GetFloat("Progress");
            set => MPBlock.SetFloat("Progress", value);
        }

        // Operators
        static public implicit operator MaterialPropertyBlock(ActionPointMatProps t)
        => t.MPBlock; 
    }
}