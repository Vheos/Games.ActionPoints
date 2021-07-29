namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    abstract public class AMatProps
    {
        // Privates
        public MaterialPropertyBlock MPBlock
        { get; private set; }

        // Constructors
        protected AMatProps()
        => MPBlock = new MaterialPropertyBlock();
    }
}