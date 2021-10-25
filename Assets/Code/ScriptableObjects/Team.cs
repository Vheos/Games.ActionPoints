namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.Extensions.General;

    [CreateAssetMenu(fileName = nameof(Team), menuName = nameof(Team), order = 4)]
    public class Team : ScriptableObject
    {
        // Inspector
        public Color _Color;

        // Public
        public IEnumerable<TeamMember> Members
        => _membersByTeam[this];           
        
        public int Count
        => _membersByTeam[this].Count;
        public Vector3 Midpoint
        {
            get
            {
                Vector3 r = Vector3.zero;
                foreach (var member in _membersByTeam[this])
                    r += member.transform.position;
                r /= _membersByTeam[this].Count;
                return r;
            }
        }
        public void AddMember(TeamMember member)
        => _membersByTeam[this].Add(member);
        public void RemoveMember(TeamMember member)
        => _membersByTeam[this].Add(member);
        public void TryInitialize()
        {
            if(!_membersByTeam.ContainsKey(this))
                _membersByTeam[this] = new List<TeamMember>();
        }

        // Privates
        static private Dictionary<Team, List<TeamMember>> _membersByTeam;

        // Initializers
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static private void StaticInitialize()
        => _membersByTeam = new Dictionary<Team, List<TeamMember>>();

    }
}