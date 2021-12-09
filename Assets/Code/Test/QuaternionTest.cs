namespace Vheos.Games.ActionPoints.Test
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Vheos.Tools.Extensions.Math;

    public class QuaternionTest : AAutoSubscriber
    {

        private void OnUpdate()
        {

        }

        protected override void DefineAutoSubscriptions()
        {
            base.DefineAutoSubscriptions();
            SubscribeTo(Get<Updatable>().OnUpdate, OnUpdate);
        }
    }
}