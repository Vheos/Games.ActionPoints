namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;

    [Serializable]
    public struct Circle
    {
        // Publics
        public Vector2 Center;
        public float Radius;

        // Initializers
        public Circle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        // Deconstructor
        public void Deconstruct(out Vector2 center, out float radius)
        {
            center = Center;
            radius = Radius;
        }
    }
}