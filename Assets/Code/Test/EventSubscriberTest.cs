namespace Vheos.Games.ActionPoints.Test
{
    using System.Collections.Generic;
    using Tools.UnityCore;
    using UnityEngine;
    using UnityEngine.Events;

    public class EventSubscriberTest : AAutoSubscriber
    {
        protected override void DefineAutoSubscriptions()
        {
            SubscribeTo(Get<EventHandlerTest>().OnPositionChanged, EventHandlerTest_LogPositionChange);
            SubscribeTo(Get<EventHandlerTest>().OnPositionChanged, (from, to) => Debug.Log($"Anonymous:\t{from} -> {to}"));
        }

        private void EventHandlerTest_LogPositionChange(Vector3 from, Vector3 to)
        {
            Debug.Log($"{Time.frameCount} / Moved:\t{from} -> {to}");
        }
        private void EventHandlerTest_LogPosition()
        {
            Debug.Log($"Stopped");
        }
    }
}