namespace Vheos.Games.ActionPoints.Test
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
        [field: SerializeField] public InputAction Log { get; private set; }
        [field: SerializeField] public Canvas Canvas { get; private set; }
        [field: SerializeField] public GameObject WorldObject { get; private set; }

        // Privates
        private void Print(InputAction.CallbackContext context)
        {
            var canvasRect = Canvas.transform as RectTransform;
            var canvasObjectRect = transform as RectTransform;
            Debug.Log($"CANVAS");
            Debug.Log($"\tPosition: {canvasRect.position}");
            Debug.Log($"\tCameraDistance: {canvasRect.DistanceTo(Canvas.worldCamera)}");
            Debug.Log($"\tPlaneDistance: {Canvas.planeDistance}");
            Debug.Log($"\tSize: {canvasRect.rect.size}");
            Debug.Log($"\tPixelSize: {Canvas.pixelRect.size}");
            Debug.Log($"\tRenderSize: {Canvas.renderingDisplaySize}");
            Debug.Log($"\tScaleFactor: {Canvas.scaleFactor}");
            Debug.Log($"");
            Debug.Log($"CANVAS OBJECT");
            Debug.Log($"\tPosition: {canvasObjectRect.position}");
            Debug.Log($"\tLocalPosition: {canvasObjectRect.localPosition}");
            Debug.Log($"\tCameraDistance: {canvasObjectRect.DistanceTo(Canvas.worldCamera)}");
            Debug.Log($"\tAnchoredPosition: {canvasObjectRect.anchoredPosition}");
            Debug.Log($"");
            Debug.Log($"WORLD OBJECT");
            Debug.Log($"\tPosition: {WorldObject.transform.position}");
            Debug.Log($"\tLocalPosition: {WorldObject.transform.localPosition}");
            Debug.Log($"\tCameraDistance: {WorldObject.DistanceTo(Canvas.worldCamera)}");

            var calculatedPoint = new Ray(Canvas.worldCamera.transform.position, Canvas.worldCamera.DirectionTowards(WorldObject)).GetPoint(Canvas.planeDistance);
            Debug.Log($"\tCalculatedPoint: {calculatedPoint }");
            Debug.Log($"\tCameraDistance: {calculatedPoint.DistanceTo(Canvas.worldCamera.transform.position)}");
            Debug.Log($"\tCalculatedPoint2: {Canvas.worldCamera.transform.position + Canvas.worldCamera.DirectionTowards(WorldObject) * Canvas.planeDistance }");

            return;
            Ray ray = new(Canvas.worldCamera.transform.position, Canvas.worldCamera.DirectionTowards(canvasObjectRect));
            Plane plane = new(Canvas.worldCamera.transform.forward, Canvas.transform.position.z);
            plane.Raycast(ray, out var distance);

            Debug.Log($"\tRay: {ray}");
            Debug.Log($"\tPlane: {plane}");
            Debug.Log($"\tRaycast: {ray.GetPoint(distance)}");
        }

        // Play
        private void Awake()
        {
            Log.Enable();
        }
        private void OnEnable()
        {
            Log.performed += Print;
        }
        private void OnDisable()
        {
            Log.performed -= Print;
        }

    }
}