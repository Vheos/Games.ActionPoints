namespace Vheos.Games.ActionPoints.Test
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;
    public class InputControl : AUpdatable
    {
        // Inspector        
        public KeyCode _ControlMode = KeyCode.Mouse1;
        public KeyCode _Forward = KeyCode.E;
        public KeyCode _Left = KeyCode.S;
        public KeyCode _Backward = KeyCode.D;
        public KeyCode _Right = KeyCode.F;
        public KeyCode _Down = KeyCode.W;
        public KeyCode _Up = KeyCode.R;
        [Range(0f, 10f)] public float _MovementSpeed = 1f;
        [Range(0f, 10f)] public float _RotationSpeed = 1f;
        public bool _RotationInvertX = false;
        public bool _RotationInvertY = true;

        // Mono
        public override void PlayUpdate()
        {
            base.PlayUpdate();
            if (_ControlMode == KeyCode.None || _ControlMode.Down())
            {
                float angleX = Input.GetAxis("Mouse Y") * (_RotationInvertY ? -1 : 1);
                float angleY = Input.GetAxis("Mouse X") * (_RotationInvertX ? -1 : 1);
                Vector3 anglesOffset = new Vector3(angleX, angleY, 0) * _RotationSpeed * 10;
                Quaternion targetRotation = transform.rotation;
                targetRotation.eulerAngles += anglesOffset * Time.deltaTime;
                transform.rotation = targetRotation;

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
        }
    }
}