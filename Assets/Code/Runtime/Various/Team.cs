namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Team : AComponentGroup<Teamable>
    {
        // Public
        static public Team Players
        { get; private set; }
        static public Team AI
        { get; private set; }
        public string Name
        { get; private set; }
        public Color Color
        { get; private set; }

        // Initializers
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static private void StaticInitialize()
        {
            Players = new Team
            {
                Name = nameof(Players),
                Color = new Color(0.5f, 0.75f, 1f, 1f),
            };
            AI = new Team()
            {
                Name = nameof(AI),
                Color = new Color(1f, 0.75f, 0.5f, 1f),
            };
        }

        // Defines
        public enum Predefined
        {
            None,
            Players,
            Enemies,
        }
    }
}