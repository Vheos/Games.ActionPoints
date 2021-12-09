namespace Vheos.Games.ActionPoints.Test
{
    using System.Collections.Generic;
    using Tools.UnityCore;
    using UnityEngine;
    using UnityEngine.Events;
    using Vheos.Tools.Extensions.General;
    using Tools.Extensions.UnityObjects;

    public class InstantiateTest : AAutoSubscriber
    {

        [SerializeField] protected bool InstantiateNew;

        private void OnUpdate()
        {
            if (InstantiateNew.Consume())
            {
                Debug.Log($"Instantiating...");
                this.CreateChildGameObject().AddComponent<Updatable>().gameObject.AddComponent<InstantiateTest>();
                //this.CreateChildComponent<InstantiateTest>();
                Debug.Log($"Assigning...");
            }
        }

        // Play
        protected override void DefineAutoSubscriptions()
        {
            SubscribeTo(Get<Updatable>().OnUpdate, OnUpdate);
        }
        protected override void PlayAwake()
        {
            base.PlayAwake();
            Debug.Log($"Awake");
        }
        protected override void PlayEnable()
        {
            base.PlayEnable();
            Debug.Log($"Enable");
        }
        protected override void PlayStart()
        {
            base.PlayStart();
            Debug.Log($"Start");
        }


    }
}