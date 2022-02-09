namespace Vheos.Games.ActionPoints.Test
{
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;
    using UnityEngine.InputSystem;

    [RequireComponent(typeof(Updatable))]
    public class ControlTransformWithInput : ABaseComponent
    {
        // Inspector        
        [field: SerializeField] public InputActionAsset InputActions { get; private set; }
        [field: SerializeField, Range(0f, 10f)] public float MovementSpeed { get; private set; } = 1f;
        [field: SerializeField, Range(0f, 10f)] public float RotationSpeed { get; private set; } = 1f;
        [field: SerializeField] public bool RotationInvertX { get; private set; } = false;
        [field: SerializeField] public bool RotationInvertY { get; private set; } = true;

        // Private
        private InputAction _control;
        private InputAction _move;
        private InputAction _rotate;
        private void ApplyInput()
        {
            if (_control.ReadValue<float>() == 0)
                return;

            // Rotation
            Vector3 anglesOffset = _rotate.ReadValue<Vector2>().YX() * RotationSpeed * 10;
            Quaternion targetRotation = transform.rotation;
            targetRotation.eulerAngles += anglesOffset * Time.deltaTime;
            transform.rotation = targetRotation;

            // Position
            var positionOffset = transform.rotation * _move.ReadValue<Vector3>() * MovementSpeed;
            transform.position = transform.position + positionOffset * Time.deltaTime;
        }

        // Defines
        private enum InputActionName
        {
            Control,
            Move,
            Rotate
        }

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            Get<Updatable>().OnUpdate.SubEnableDisable(this, ApplyInput);
            _control = InputActions.FindAction(nameof(InputActionName.Control));
            _move = InputActions.FindAction(nameof(InputActionName.Move));
            _rotate = InputActions.FindAction(nameof(InputActionName.Rotate));

            _control.Enable();
            _move.Enable();
            _rotate.Enable();
        }
    }
}