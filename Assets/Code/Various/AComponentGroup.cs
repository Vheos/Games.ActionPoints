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
        public IReadOnlyList<T> Members
        => _members;
        public int Count
        => _members.Count;
        public Vector3 Midpoint
        => _members.Midpoint();
        virtual public void TryAddMember(T member)
        {
            if (_members.TryAddUnique(member))
                OnMembersChanged?.Invoke();
        }
        virtual public void TryRemoveMember(T member)
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