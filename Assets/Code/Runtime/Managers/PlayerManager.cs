namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Tools.UnityCore;
    using Tools.Extensions.General;
    using Tools.Extensions.Collections;

    [DisallowMultipleComponent]
    public class PlayerManager : AComponentManager<PlayerManager, Player>
    {
        // Events
        static public AutoEvent<Player> OnPlayerJoin
        { get; private set; }
        static public AutoEvent<Player> OnPlayerLeave
        { get; private set; }

        // Inspector
        [SerializeField] protected InputActionAsset _Actions;
        [SerializeField] protected InputActionReference _JoinAction;
        [SerializeField] protected Color[] _PlayerColors;

        // Private
        static private InputAction _joinAction;
        static private bool TryGetControlScheme(string name, out InputControlScheme controlScheme)
        => name.IsNotNullOrEmpty()
        && _instance._Actions.controlSchemes.TryFind(t => t.name == name, out controlScheme);
        static private bool TryGetControlScheme(InputDevice device, out InputControlScheme controlScheme)
        => device != null
        && InputControlScheme.FindControlSchemeForDevice(device, _instance._Actions.controlSchemes).TryNonNull(out controlScheme);
        static private IEnumerable<InputControlScheme> GetControlSchemes(InputAction inputAction)
        {
            if (inputAction.GetBindingForControl(inputAction.activeControl).TryNonNull(out var binding))
                foreach (var controlSchemeName in binding.groups.Split(';'))
                    if (TryGetControlScheme(controlSchemeName, out var controlScheme))
                        yield return controlScheme;
        }
        static private IEnumerable<InputControlScheme> GetControlSchemes(params InputDevice[] devices)
        {
            foreach (var controlScheme in _instance._Actions.controlSchemes)
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
            _PlayerColors.TryGet(playerID, out var playerColor, Color.white);
            InstantiateComponent().Initialize(_Actions, context.control.device, controlScheme, playerID, playerColor);
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

            _joinAction = new InputAction(nameof(_joinAction), _JoinAction.action.type);
            foreach (var binding in _JoinAction.action.bindings)
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