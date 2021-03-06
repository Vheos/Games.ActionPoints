namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Games.Core;
    using Tools.Extensions.General;
    using Tools.Extensions.Collections;

    [DisallowMultipleComponent]
    public class PlayerManager : AStaticManager<PlayerManager, Player>
    {
        // Events
        static public AutoEvent<Player> OnPlayerJoin
        { get; private set; }
        static public AutoEvent<Player> OnPlayerLeave
        { get; private set; }

        // Inspector
        [field: SerializeField] public InputActionAsset Actions { get; private set; }
        [field: SerializeField] public InputActionReference JoinAction { get; private set; }
        [field: SerializeField] public Color[] PlayerColors { get; private set; }

        // Private
        static private InputAction _joinAction;
        static private bool TryGetControlScheme(string name, out InputControlScheme controlScheme)
        => name.IsNotNullOrEmpty()
        && _instance.Actions.controlSchemes.TryFind(t => t.name == name, out controlScheme);
        static private bool TryGetControlScheme(InputDevice device, out InputControlScheme controlScheme)
        => device != null
        && InputControlScheme.FindControlSchemeForDevice(device, _instance.Actions.controlSchemes).TryNonNull(out controlScheme);
        static private IEnumerable<InputControlScheme> GetControlSchemes(InputAction inputAction)
        {
            if (inputAction.GetBindingForControl(inputAction.activeControl).TryNonNull(out var binding))
                foreach (var controlSchemeName in binding.groups.Split(';'))
                    if (TryGetControlScheme(controlSchemeName, out var controlScheme))
                        yield return controlScheme;
        }
        static private IEnumerable<InputControlScheme> GetControlSchemes(params InputDevice[] devices)
        {
            foreach (var controlScheme in _instance.Actions.controlSchemes)
            {
                var match = controlScheme.PickDevicesFrom(devices);
                if (!match.hasMissingRequiredDevices)
                    yield return controlScheme;
                match.Dispose();
            }
        }
        static private int FindAvailableID()
        {
            int r = 0;
            while (_components.Any(t => t.ID == r))
                r++;
            return r;
        }
        private void OnInputJoin(InputAction.CallbackContext context)
        {
            if (!GetControlSchemes(context.action).TryGetAny(out var controlScheme)
            || _components.Any
                (
                    t => t.IsUsingControlScheme(controlScheme)
                      && t.IsUsingDevice(context.control.device)
                ))
                return;

            var playerID = FindAvailableID();
            PlayerColors.TryGet(playerID, out var playerColor, Color.white);
            var newPlayer = InstantiateComponent();
            newPlayer.Initialize(Actions, context.control.device, controlScheme, playerID, playerColor);
            newPlayer.OnPlayDestroy.SubOnce(() => OnPlayerLeave.Invoke(newPlayer));
            OnPlayerJoin.Invoke(newPlayer);
        }

        // Initializers
        [SuppressMessage("CodeQuality", "IDE0051")]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static private void StaticInitialize()
        {
            OnPlayerJoin = new AutoEvent<Player>();
            OnPlayerLeave = new AutoEvent<Player>();
        }

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();

            _joinAction = new InputAction(nameof(_joinAction), InputActionType.PassThrough);
            foreach (var binding in JoinAction.action.bindings)
                _joinAction.AddBinding(binding);
            _joinAction.Enable();
        }
        protected override void PlayEnable()
        {
            base.PlayEnable();
            _joinAction.performed += OnInputJoin;
        }
        protected override void PlayDisable()
        {
            base.PlayDisable();
            _joinAction.performed -= OnInputJoin;
        }
    }
}