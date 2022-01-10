namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.UtilityN;
    using Tools.Extensions.Collections;
    using Tools.Extensions.General;

    [DisallowMultipleComponent]
    public class CameraManager : AComponentManager<CameraManager, Camera>
    {
        // Inspector
        [SerializeField] protected GameObject _CameraPrefab;

        // Publics
        static public Camera CursorCamera
        {
            get
            {
                if (_cursorCameraDirty
                && _cursorCameraLocks.IsEmpty()
                && TryFindCursorCamera(out _cursorCamera))
                    _cursorCameraDirty = false;

                return _cursorCamera;
            }
        }
        static public void LockCursorCamera(Behaviour owner)
        => _cursorCameraLocks.TryAddUnique(owner);
        static public void UnlockCursorCamera(Behaviour owner)
        => _cursorCameraLocks.TryRemove(owner);

        // Privates
        static private Camera _cursorCamera;
        static private HashSet<Behaviour> _cursorCameraLocks;
        static private bool _cursorCameraDirty;
        static private bool TryFindCursorCamera(out Camera cursorCamera)
        {
            foreach (var camera in _components)
                if (camera != null
                && camera.isActiveAndEnabled
                && camera.pixelRect.Contains(Input.mousePosition))
                {
                    cursorCamera = camera;
                    return true;
                }

            cursorCamera = null;
            return false;
        }
        static private void TryCreateFirstCamera(GameObject prefab)
        {
            if (_components.IsNotEmpty())
                return;

            Camera newCamera;
            if (prefab != null)
            {
                newCamera = GameObject.Instantiate(prefab).GetComponent<Camera>();
                _components.Add(newCamera);
            }
            else
                newCamera = AddComponentTo(new GameObject());

            newCamera.name = "Camera";
        }

        // Play
        protected override void DefineAutoSubscriptions()
        {
            base.DefineAutoSubscriptions();
            SubscribeTo(CursorManager.OnMoveCursor, (from, to) => _cursorCameraDirty = true);
        }
        protected override void PlayAwake()
        {
            base.PlayAwake();

            TryCreateFirstCamera(_CameraPrefab);

            _cursorCamera = FirstActive;
            _cursorCameraLocks = new HashSet<Behaviour>();
            _cursorCameraDirty = true;
        }
    }
}