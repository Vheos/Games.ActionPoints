namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Vheos.Tools.Extensions.UnityObjects;
    using UnityEngine.InputSystem;

    public class Character : AAutoSubscriber
    {
        private InputAction _confirm;
        private InputAction _moveCursor;

        protected override void DefineAutoSubscriptions()
        {
            base.DefineAutoSubscriptions();
            SubscribeAuto(Get<Movable>().OnStartMoving, (from) => Debug.Log($"{Time.time:F2}\tStart\t{from}"));
            SubscribeAuto(Get<Movable>().OnMove, (from, to) => Debug.Log($"{Time.time:F2}\tMove\t{from}\t{to}"));
            SubscribeAuto(Get<Movable>().OnStop, (to) => Debug.Log($"{Time.time:F2}\tStop\t{to}"));
        }

        private void HandleInputActions(InputAction.CallbackContext context)
        {
            if (context.action == _confirm)
            {
  
            }
            else if (context.action == _moveCursor)
            {
        
            }
        }

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            Get<PlayerInput>().onActionTriggered += HandleInputActions;
            _confirm = Get<PlayerInput>().currentActionMap.FindAction(nameof(_confirm));
            _moveCursor = Get<PlayerInput>().currentActionMap.FindAction(nameof(_moveCursor));
        }
    }
}