namespace Vheos.Tools.UnityCore
{
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Vheos.Tools.Extensions.UnityObjects;
    using Vheos.Tools.Extensions.Math;

    [RequireComponent(typeof(RectTransform))]
    public class CanvasTest : MonoBehaviour
    {
        // Inspector
        [SerializeField] protected InputAction _Log;
        [SerializeField] protected Canvas _Canvas;
        [SerializeField] protected GameObject _WorldObject;

        // Privates
        private void Log(InputAction.CallbackContext context)
        {
            var canvasRect = _Canvas.transform as RectTransform;
            var canvasObjectRect = transform as RectTransform;
            Debug.Log($"CANVAS");
            Debug.Log($"\tPosition: {canvasRect.position}");
            Debug.Log($"\tCameraDistance: {canvasRect.DistanceTo(_Canvas.worldCamera)}");
            Debug.Log($"\tPlaneDistance: {_Canvas.planeDistance}");
            Debug.Log($"\tSize: {canvasRect.rect.size}");
            Debug.Log($"\tPixelSize: {_Canvas.pixelRect.size}");
            Debug.Log($"\tRenderSize: {_Canvas.renderingDisplaySize}");
            Debug.Log($"\tScaleFactor: {_Canvas.scaleFactor}");
            Debug.Log($"");
            Debug.Log($"CANVAS OBJECT");
            Debug.Log($"\tPosition: {canvasObjectRect.position}");
            Debug.Log($"\tLocalPosition: {canvasObjectRect.localPosition}");
            Debug.Log($"\tCameraDistance: {canvasObjectRect.DistanceTo(_Canvas.worldCamera)}");
            Debug.Log($"\tAnchoredPosition: {canvasObjectRect.anchoredPosition}");
            Debug.Log($"");
            Debug.Log($"WORLD OBJECT");
            Debug.Log($"\tPosition: {_WorldObject.transform.position}");
            Debug.Log($"\tLocalPosition: {_WorldObject.transform.localPosition}");
            Debug.Log($"\tCameraDistance: {_WorldObject.DistanceTo(_Canvas.worldCamera)}");

            var calculatedPoint = new Ray(_Canvas.worldCamera.transform.position, _Canvas.worldCamera.DirectionTowards(_WorldObject)).GetPoint(_Canvas.planeDistance);
            Debug.Log($"\tCalculatedPoint: {calculatedPoint }");
            Debug.Log($"\tCameraDistance: {calculatedPoint.DistanceTo(_Canvas.worldCamera.transform.position)}");
            Debug.Log($"\tCalculatedPoint2: {_Canvas.worldCamera.transform.position + _Canvas.worldCamera.DirectionTowards(_WorldObject) * _Canvas.planeDistance }");

            return;
            Ray ray = new(_Canvas.worldCamera.transform.position, _Canvas.worldCamera.DirectionTowards(canvasObjectRect));
            Plane plane = new(_Canvas.worldCamera.transform.forward, _Canvas.transform.position.z);
            plane.Raycast(ray, out var distance);

            Debug.Log($"\tRay: {ray}");
            Debug.Log($"\tPlane: {plane}");
            Debug.Log($"\tRaycast: {ray.GetPoint(distance)}");
        }

        // Play
        private void Awake()
        {
            _Log.Enable();
        }
        private void OnEnable()
        {
            _Log.performed += Log;
        }
        private void OnDisable()
        {
            _Log.performed -= Log;
        }

    }
}