namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Games.Core;
    using UnityEngine.InputSystem.Users;

    [RequireComponent(typeof(Updatable))]
    [DisallowMultipleComponent]
    public class Player : ABaseComponent
    {
        // Events
        public AutoEvent OnInputPressConfirm
        { get; } = new AutoEvent();
        public AutoEvent OnInputReleaseConfirm
        { get; } = new AutoEvent();
        public AutoEvent<Vector2> OnInputMoveCursor
        { get; } = new AutoEvent<Vector2>();
        private void InvokeOnInputMoveCursor()
        {
            var delta = _moveCursor.ReadValue<Vector2>();
            if (delta != Vector2.zero)
            {
                if (_deltaTimeDependentMoveCursor)
                    delta *= Time.deltaTime;
                OnInputMoveCursor.Invoke(delta);
            }
        }
        private void InvokeOnInputPressConfirm(InputAction.CallbackContext context)
        => OnInputPressConfirm.Invoke();
        private void InvokeOnInputReleaseConfirm(InputAction.CallbackContext context)
        => OnInputReleaseConfirm.Invoke();

        // Publics   
        public int ID
        { get; private set; }
        public bool IsUsingControlScheme(InputControlScheme controlScheme)
        => _inputUser.controlScheme == controlScheme;
        public bool IsUsingDevice(InputDevice device)
        => _inputUser.pairedDevices.Contains(device);

        // Privates
        private InputUser _inputUser;
        private InputAction _confirm;
        private InputAction _moveCursor;
        private bool _deltaTimeDependentMoveCursor;
        private void TrySubscribeInputActions()
        {
            if (!_inputUser.valid)
                return;

            _confirm.performed += InvokeOnInputPressConfirm;
            _confirm.canceled += InvokeOnInputReleaseConfirm;
        }
        private void TryUnsubscribeInputActions()
        {
            if (!_inputUser.valid)
                return;

            _confirm.performed -= InvokeOnInputPressConfirm;
            _confirm.canceled -= InvokeOnInputReleaseConfirm;
        }
        private void EnableActions()
        {
            foreach (var action in _inputUser.actions)
                action.Enable();
        }
        private void DisableActions()
        {
            foreach (var action in _inputUser.actions)
                action.Disable();
        }

        // Play
        public void Initialize(InputActionAsset actionAsset, InputDevice device, InputControlScheme controlScheme, int id, Color cursorColor)
        {
            ID = id;
            var newActions = Instantiate(actionAsset);
            _inputUser = InputUser.PerformPairingWithDevice(device);
            _inputUser.AssociateActionsWithUser(newActions);
            _inputUser.ActivateControlScheme(controlScheme);

            _confirm = newActions.FindAction(nameof(InputActionEnum.Confirm));
            _moveCursor = newActions.FindAction(nameof(InputActionEnum.MoveCursor));
            _deltaTimeDependentMoveCursor = !_inputUser.pairedDevices.Any(t => t is Pointer);

            Get<Updatable>().OnUpdate.SubscribeAuto(this, InvokeOnInputMoveCursor);
            TrySubscribeInputActions();
            EnableActions();

            name = $"{nameof(Player)}{ID + 1}";
            var newCursor = UICursorManager.InstantiateComponent();
            newCursor.Initialize(CanvasManager.Any, CameraManager.Any);
            newCursor.BindToPlayer(this, cursorColor);

            PlayerManager.OnPlayerJoin.Invoke(this);
        }
        protected override void PlayEnable()
        {
            base.PlayEnable();
            TrySubscribeInputActions();
        }
        protected override void PlayDisable()
        {
            base.PlayDisable();
            TryUnsubscribeInputActions();
        }
        protected override void PlayDestroy()
        {
            base.PlayDestroy();
            DisableActions();
            _inputUser.UnpairDevicesAndRemoveUser();
            PlayerManager.OnPlayerLeave.Invoke(this);
        }
    }
}