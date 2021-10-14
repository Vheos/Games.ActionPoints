namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;

    public class Player : AUpdatable
    {
        // Inspector
        public Controller _Controller;
        public Color _Color;
        


        // Enum
        public enum Controller
        {
            Mouse,
            Gamepad,
        }
    }
}