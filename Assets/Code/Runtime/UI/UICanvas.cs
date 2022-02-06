namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using Games.Core;
    using Tools.Extensions.Math;
    using UnityCamera = UnityEngine.Camera;

    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [RequireComponent(typeof(RectResizeable))]
    [DisallowMultipleComponent]
    public class UICanvas : ABaseComponent
    {
        // Publics
        public CCamera WorldCamera
        { get; private set; }
        private CCamera _canvasCamera;
        public CCamera CanvasCamera
        {
            get => _canvasCamera;
            private set
            {
                _canvasCamera = value;
                Get<Canvas>().worldCamera = _canvasCamera.Unity;
            }
        }
        public Vector2 Size
        => Get<RectTransform>().rect.size;
        public float PlaneDistance
        => Get<Canvas>().planeDistance;
        public float ScaleFactor
        => Get<Canvas>().scaleFactor;

        // Position
        public CCamera GetCameraFor(GameObject t)
        => t.IsOnLayer(BuiltInLayer.UI) ? CanvasCamera : WorldCamera;
        public CCamera GetCameraFor(Component t)
        => GetCameraFor(t.gameObject);
        public Vector2 CanvasPosition(GameObject t)
        => t.IsOnLayer(BuiltInLayer.UI)
            ? t.transform.position
            : WorldCamera.Unity.WorldToScreenPoint(t.transform.position) / ScaleFactor;
        public Vector2 CanvasPosition(Component t)
        => CanvasPosition(t.gameObject);
        public Vector2 ScreenPosition(GameObject t)
        => t.IsOnLayer(BuiltInLayer.UI)
            ? t.transform.position * ScaleFactor
            : WorldCamera.Unity.WorldToScreenPoint(t.transform.position);
        public Vector2 ScreenPosition(Component t)
        => ScreenPosition(t.gameObject);
        public Vector2 CanvasToScreenPosition(Vector2 t)
        => t * ScaleFactor;
        public Vector2 ScreenToCanvasPosition(Vector2 t)
        => t / ScaleFactor;

        // Privates
        private void UpdateCanvasCamera(Vector2 from, Vector2 to)
        {
            CanvasCamera.transform.position = to.Div(2f).Append(-PlaneDistance);
            CanvasCamera.Unity.orthographicSize = to.y / 2f;
        }

        // Play
        protected override void PlayStart()
        {
            base.PlayStart();
            WorldCamera = CameraManager.Any;
            CanvasCamera = UICanvasManager.InstantiateUICamera();

            Get<RectResizeable>().OnResize.SubEnableDisable(this, UpdateCanvasCamera);
            UpdateCanvasCamera(default, Size);
        }
    }
}