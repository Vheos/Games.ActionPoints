namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.Extensions.Collections;
    using Tools.UtilityN;
    using Tools.Extensions.General;

    public class CameraManager : AComponentManager<Camera>
    {
        // Publics
        static public Camera CursorCamera
        {
            get
            {
                if (_cursorCameraDirty
                && _cursorCameraLocks.IsEmpty()
                && FindCursorCamera().TryNonNull(out var newCursorCamera))
                {
                    _cursorCamera = newCursorCamera;
                    _cursorCameraDirty = false;
                }

                return _cursorCamera;
            }
        }
        static public void SetDirtyCursorCamera(Vector2 from, Vector2 to)
        => _cursorCameraDirty = true;
        static public void LockCursorCamera(Behaviour owner)
        => _cursorCameraLocks.TryAddUnique(owner);
        static public void UnlockCursorCamera(Behaviour owner)
        => _cursorCameraLocks.TryRemove(owner);

        // Privates
        static private Camera _cursorCamera;
        static private List<Behaviour> _cursorCameraLocks;
        static private bool _cursorCameraDirty;
        static private Camera FindCursorCamera()
        {
            foreach (var camera in _components)
                if (camera == null)
                    _instance.StartCoroutine(Coroutines.AfterCurrentTimestep(() => _components.Remove(camera)));
                else if (camera.isActiveAndEnabled
                && camera.pixelRect.Contains(Input.mousePosition))
                    return camera;
            return null;
        }

        // Play
        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            SubscribeTo(CursorManager.OnCursorMoved, SetDirtyCursorCamera);
        }
        protected override void PlayAwake()
        {
            base.PlayAwake();
            _cursorCamera = FirstActive;
            _cursorCameraLocks = new List<Behaviour>();
        }
    }
}