namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Team
    {
        // Public
        static public Team Players
        { get; private set; }
        static public Team Enemies
        { get; private set; }
        public string Name
        { get; private set; }
        public Color Color
        { get; private set; }
        public IEnumerable<Teamable> Members
        => _members;
        public int Count
        => _members.Count;
        public Vector3 Midpoint
        {
            get
            {
                Vector3 r = Vector3.zero;
                foreach (var member in _members)
                    r += member.transform.position;
                r /= _members.Count;
                return r;
            }
        }
        public void AddMember(Teamable member)
        => _members.Add(member);
        public void RemoveMember(Teamable member)
        => _members.Add(member);

        // Privates
        private List<Teamable> _members;

        // Initializers
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static private void StaticInitialize()
        {
            Players = new Team
            {
                Name = nameof(Players),
                Color = new Color(0.5f, 0.75f, 1f, 1f),
            };
            Enemies = new Team()
            {
                Name = nameof(Enemies),
                Color = new Color(1f, 0.75f, 0.5f, 1f),
            };
        }
        private Team()
        => _members = new List<Teamable>();
    }
}