namespace Vheos.Tools.UnityCore
{
    using System;

    abstract public class AEvent
    {
        // Public
        public int ActionsCount
        => InternalEvent == null ? 0 : InternalEvent.GetInvocationList().Length;

        // Privates
        abstract internal void Unsubscribe(AEventSubscriber component);
        abstract protected Delegate InternalEvent
        { get; }

        // Initializers
        internal AEvent()
        { }
    }
}