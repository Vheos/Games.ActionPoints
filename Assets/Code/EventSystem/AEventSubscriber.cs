namespace Vheos.Tools.UnityCore
{
    using System;
    using System.Collections.Generic;

    abstract public class AEventSubscriber : ABaseComponent
    {
        // Publics
        virtual protected void SubscribeToEvents()
        { }
        public void SubscribeTo(Event @event, Action action)
        {
            @event.Subscribe(action);
            _subscribedEvents.Add(@event);
        }
        public void SubscribeTo(Event @event, params Action[] actions)
        => SubscribeTo(@event, Delegate.Combine(actions) as Action);
        public void SubscribeTo<T1>(Event<T1> @event, Action<T1> action)
        {
            @event.Subscribe(action);
            _subscribedEvents.Add(@event);
        }
        public void SubscribeTo<T1>(Event @event, params Action<T1>[] actions)
        => SubscribeTo(@event, Delegate.Combine(actions) as Action<T1>);
        public void SubscribeTo<T1, T2>(Event<T1, T2> @event, Action<T1, T2> action)
        {
            @event.Subscribe(action);
            _subscribedEvents.Add(@event);
        }
        public void SubscribeTo<T1, T2>(Event @event, params Action<T1, T2>[] actions)
        => SubscribeTo(@event, Delegate.Combine(actions) as Action<T1, T2>);

        // Privates
        private readonly HashSet<AEvent> _subscribedEvents = new HashSet<AEvent>();
        private void UnsubscribeFromEvents()
        {
            foreach (var @event in _subscribedEvents)
                @event.Unsubscribe(this);
            _subscribedEvents.Clear();
        }

        // Play
        protected override void PlayEnable()
        {
            base.PlayEnable();
            SubscribeToEvents();
        }
        protected override void PlayDisable()
        {
            base.PlayDisable();
            UnsubscribeFromEvents();
        }
    }
}