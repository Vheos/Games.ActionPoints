namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class CameraExtended : APlayable
    {
        // Publics
        static public CameraExtended Main;
        public Ray CursorRay
        => _camera.ScreenPointToRay(Input.mousePosition);
        public Plane ScreenPlane(Vector3 worldPoint)
        => new Plane(transform.forward.Neg(), worldPoint);
        public Vector3 CursorToWorldPoint(float distanceFromCamera)
        => _camera.ScreenToWorldPoint(Input.mousePosition.XY().Append(distanceFromCamera));
        public Vector3 CursorToPlanePoint(Plane plane)
        {
            Ray ray = CursorRay;
            if (plane.Raycast(ray, out float distance))
                return ray.GetPoint(distance);
            return float.NaN.ToVector3();
        }
        public Vector3 CursorToScreenPlanePoint(Vector3 worldPoint)
        => CursorToPlanePoint(ScreenPlane(worldPoint));

        // Privates
        private Camera _camera;

        // Mono
        override public void PlayAwake()
        {
            base.PlayAwake();
            _camera = GetComponent<Camera>();
            Main = this;
        }
    }
}