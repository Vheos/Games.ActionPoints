namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;

    [Serializable]
    public struct Segment2D
    {
        // Publics
        public Vector2 From;
        public Vector2 To;

        // Initializers
        public Segment2D(Vector2 from, Vector2 to)
        {
            From = from;
            To = to;
        }

        // Deconstructor
        public void Deconstruct(out Vector2 from, out Vector2 to)
        {
            from = From;
            to = To;
        }
    }
}