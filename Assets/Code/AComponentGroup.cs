namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.Extensions.Collections;

    abstract public class AComponentGroup<T> where T : Component
    {
        public IEnumerable<T> Members
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
        virtual public void AddMember(T member)
        => _members.TryAddUnique(member);
        virtual public void RemoveMember(T member)
        => _members.Remove(member);

        // Privates
        protected List<T> _members;

        // Initializers
        protected AComponentGroup()
        => _members = new List<T>();
    }
}