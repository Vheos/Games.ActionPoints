namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.Math;
    public class TestMousable : AMousable
    {
        // Inspector
        [SerializeField] protected MouseManager.Button __Button;
        [SerializeField] [Range(0f, 1f)] protected float __LerpHalfTime;

        // Privates
        private bool _isAttached;
        private Plane _attachmentPlane;
        private Vector3 _attachmentOffset;
        private Vector3 _targetScale;

        // Mouse
        public override void MouseGainHighlight()
        {
            base.MouseGainHighlight();
            _targetScale = 1.5f.ToVector3();
        }
        public override void MouseLoseHighlight()
        {
            base.MouseLoseHighlight();
            _targetScale = 1f.ToVector3();
        }
        public override void MousePress(MouseManager.Button button, Vector3 worldPoint)
        {
            base.MousePress(button, worldPoint);
            if (button != __Button)
                return;

            _attachmentPlane = CameraManager.CursorCamera.ScreenPlane(worldPoint);
            _attachmentOffset = transform.position.OffsetTo(worldPoint);
            _isAttached = true;
            CameraManager.LockCursorCamera(this);
        }
        public override void MouseRelease(MouseManager.Button button)
        {
            base.MouseRelease(button);
            if (button != __Button)
                return;

            _isAttached = false;
            CameraManager.UnlockCursorCamera(this);
        }
        public override bool IsHighlightLocked
        => _isAttached;

        // Mono
        public override void PlayAwake()
        {
            base.PlayAwake();
            _targetScale = transform.localScale;
        }
        public override void PlayUpdate()
        {
            base.PlayUpdate();
            float lerpAlpha = NewUtility.LerpHalfTimeToAlpha(__LerpHalfTime);
            transform.localScale = transform.localScale.Lerp(_targetScale, lerpAlpha);
            if (_isAttached)
            {
                Vector3 targetPosition = CameraManager.CursorCamera.CursorToPlanePoint(_attachmentPlane) - _attachmentOffset;
                transform.position = transform.position.Lerp(targetPosition, lerpAlpha);
            }
        }

#if UNITY_EDITOR
        public override void EditAdd()
        {
            base.EditAdd();
            __Button = MouseManager.Button.Left;
            __LerpHalfTime = 0.1f;
        }
#endif
    }
}