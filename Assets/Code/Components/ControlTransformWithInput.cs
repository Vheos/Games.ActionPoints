namespace Vheos.Games.ActionPoints.Test
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;

    public class ControlTransformWithInput : AEventSubscriber
    {
        // Inspector        
        [SerializeField] protected KeyCode _ControlMode = KeyCode.Mouse1;
        [SerializeField] protected KeyCode _Forward = KeyCode.E;
        [SerializeField] protected KeyCode _Left = KeyCode.S;
        [SerializeField] protected KeyCode _Backward = KeyCode.D;
        [SerializeField] protected KeyCode _Right = KeyCode.F;
        [SerializeField] protected KeyCode _Down = KeyCode.W;
        [SerializeField] protected KeyCode _Up = KeyCode.R;
        [SerializeField] [Range(0f, 10f)] protected float _MovementSpeed = 1f;
        [SerializeField] [Range(0f, 10f)] protected float _RotationSpeed = 1f;
        [SerializeField] protected bool _RotationInvertX = false;
        [SerializeField] protected bool _RotationInvertY = true;

        // Private
        private void ApplyInput()
        {
            if (_ControlMode != KeyCode.None && _ControlMode.Up())
                return;

            // Rotation
            float angleX = Input.GetAxis("Mouse Y") * (_RotationInvertY ? -1 : 1);
            float angleY = Input.GetAxis("Mouse X") * (_RotationInvertX ? -1 : 1);
            Vector3 anglesOffset = new Vector3(angleX, angleY, 0) * _RotationSpeed * 10;
            Quaternion targetRotation = transform.rotation;
            targetRotation.eulerAngles += anglesOffset * Time.deltaTime;
            transform.rotation = targetRotation;

            // Position
            Vector3 positionOffset = Vector3.zero;
            if (_Forward.Down()) positionOffset += Vector3.forward;
            if (_Left.Down()) positionOffset += Vector3.left;
            if (_Backward.Down()) positionOffset += Vector3.back;
            if (_Right.Down()) positionOffset += Vector3.right;
            if (_Down.Down()) positionOffset += Vector3.down;
            if (_Up.Down()) positionOffset += Vector3.up;
            positionOffset = transform.rotation * positionOffset * _MovementSpeed;
            transform.position = transform.position + positionOffset * Time.deltaTime;
        }

        // Play
        protected override void AutoSubscribeToEvents()
        {
            base.AutoSubscribeToEvents();
            SubscribeTo(Get<Updatable>().OnUpdated, ApplyInput);
        }
    }
}