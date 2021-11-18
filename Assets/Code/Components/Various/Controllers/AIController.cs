namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;

    public class AIController : AController
    {
        // Inspector
        [SerializeField] protected Action _Action;
        [SerializeField] [Range(0f, 1f)] protected float _TargetingTime;

        // Private
        private void OnUpdate()
        {
            Combatable combatable = Get<Combatable>();
            
        }

        // Play
        protected override void DefineAutoSubscriptions()
        {
            base.DefineAutoSubscriptions();
            SubscribeTo(Get<Updatable>().OnUpdated, OnUpdate);
        }
    }
}