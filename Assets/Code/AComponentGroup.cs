namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.Extensions.Collections;
    using Event = Tools.UnityCore.Event;

    abstract public class AComponentGroup<T> where T : Component
    {
        // Events
        public Event OnMembersChanged
        { get; } = new Event();

        // Publics
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
        {
            if (_members.TryAddUnique(member))
                OnMembersChanged?.Invoke();
        }
        virtual public void RemoveMember(T member)
        {
            if (_members.Remove(member))
                OnMembersChanged?.Invoke();
        }

        // Privates
        protected List<T> _members;

        // Initializers
        protected AComponentGroup()
        => _members = new List<T>();
    }
}