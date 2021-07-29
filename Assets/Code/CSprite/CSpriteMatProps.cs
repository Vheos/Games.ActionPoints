namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    public class CSpriteMatProps : AMatProps
    {
        // Publics
        public Texture Texture
        {
            get => MPBlock.GetTexture("Texture");
            set => MPBlock.SetTexture("Texture", value);
        }
        public Vector4 TextureST
        {
            get => MPBlock.GetVector("Texture_ST");
            set => MPBlock.SetVector("Texture_ST", value);
        }
        public Color WhiteTint
        {
            get => MPBlock.GetColor("WhiteTint");
            set => MPBlock.SetColor("WhiteTint", value);
        }
        public Color BlackTint
        {
            get => MPBlock.GetColor("BlackTint");
            set => MPBlock.SetColor("BlackTint", value);
        }

        // Operators
        static public implicit operator MaterialPropertyBlock(CSpriteMatProps t)
        => t.MPBlock;
    }
}