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
        [SerializeField] protected InputActionAsset _InputActions;
        [SerializeField] [Range(0f, 10f)] protected float _MovementSpeed = 1f;
        [SerializeField] [Range(0f, 10f)] protected float _RotationSpeed = 1f;
        [SerializeField] protected bool _RotationInvertX = false;
        [SerializeField] protected bool _RotationInvertY = true;

        // Private
        private InputAction _control;
        private InputAction _move;
        private InputAction _rotate;
        private void ApplyInput()
        {
            if (_control.ReadValue<float>() == 0)
                return;

            // Rotation
            Vector3 anglesOffset = _rotate.ReadValue<Vector2>().YX() * _RotationSpeed * 10;
            Quaternion targetRotation = transform.rotation;
            targetRotation.eulerAngles += anglesOffset * Time.deltaTime;
            transform.rotation = targetRotation;

            // Position
            var positionOffset = transform.rotation * _move.ReadValue<Vector3>() * _MovementSpeed;
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
            Get<Updatable>().OnUpdate.SubscribeAuto(this, ApplyInput);
            _control = _InputActions.FindAction(nameof(InputActionName.Control));
            _move = _InputActions.FindAction(nameof(InputActionName.Move));
            _rotate = _InputActions.FindAction(nameof(InputActionName.Rotate));

            _control.Enable();
            _move.Enable();
            _rotate.Enable();
        }
    }
}