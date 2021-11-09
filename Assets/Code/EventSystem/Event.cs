namespace Vheos.Tools.UnityCore
{
    using System;

    sealed public class Event : AEvent
    {
        // Publics
        internal void Invoke()
        => _internalEvent?.Invoke();

        // Privates
        private Action _internalEvent;
        protected override Delegate InternalEvent
        => _internalEvent;
        internal void Subscribe(Action action)
        => _internalEvent += action;
        internal override void Unsubscribe(AEventSubscriber subscriber)
        {
            foreach (var action in _internalEvent.GetInvocationList())
                if ((object)action.Target == subscriber)
                    _internalEvent -= action as Action;
        }
    }

    sealed public class Event<T1> : AEvent
    {
        // Publics
        public void Invoke(T1 arg1)
        => _internalEvent?.Invoke(arg1);

        // Privates
        private Action<T1> _internalEvent;
        protected override Delegate InternalEvent
        => _internalEvent;
        internal void Subscribe(Action<T1> action)
        => _internalEvent += action;
        internal override void Unsubscribe(AEventSubscriber subscriber)
        {
            foreach (var action in _internalEvent.GetInvocationList())
                if ((object)action.Target == subscriber)
                    _internalEvent -= action as Action<T1>;
        }
    }

    sealed public class Event<T1, T2> : AEvent
    {
        // Publics
        public void Invoke(T1 arg1, T2 arg2)
        => _internalEvent?.Invoke(arg1, arg2);

        // Privates
        private Action<T1, T2> _internalEvent;
        protected override Delegate InternalEvent
        => _internalEvent;
        internal void Subscribe(Action<T1, T2> action)
        => _internalEvent += action;
        internal override void Unsubscribe(AEventSubscriber subscriber)
        {
            foreach (var action in _internalEvent.GetInvocationList())
                if ((object)action.Target == subscriber)
                    _internalEvent -= action as Action<T1, T2>;
        }
    }
}