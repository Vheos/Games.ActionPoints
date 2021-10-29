namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Collections;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;
    using Tools.Extensions.General;

    [DefaultExecutionOrder(-1)]
    [DisallowMultipleComponent]
    public class CursorManager : ABaseComponent
    {
        // Inspector
        [SerializeField] protected Button[] _Buttons;
        [SerializeField] protected GameObject _CursorPrefab;

        // Publics
        static public Transform CursorTransform
        { get; private set; }
        static public Mousable CursorMousable
        { get; private set; }
        static public RaycastHit CursorMousableHitInfo
        { get; private set; }
        static public void SetCursorDistance(float distance)
        => _cursorDistance = distance;
        static public void SetCursorDistance(Vector3 worldPoint)
        => _cursorDistance = CameraManager.FirstActive.transform.position.DistanceTo(worldPoint);
        static public void SetCursorDistance(Transform transform)
        => _cursorDistance = CameraManager.FirstActive.DistanceTo(transform);

        // Privates
        static private Dictionary<Button, Mousable> _lockedMousablesByButton;
        static private Mousable _previousCursorMousable;
        static private Vector3 _previousMousePosition;
        static private float _cursorDistance;
        static private void CreateCursorTransform(GameObject prefab, Component parent)
        {
            if (prefab != null)
            {
                Cursor.visible = false;
                CursorTransform = Instantiate(prefab).transform;
            }
            else
                CursorTransform = new GameObject().transform;

            CursorTransform.name = nameof(CursorTransform);
            CursorTransform.BecomeChildOf(parent);
        }
        static private void UpdateCursorMousable()
        {
            foreach (var hitInfo in NewUtility.RaycastFromCamera(CameraManager.FirstActive, LayerMask.GetMask(nameof(Mousable)), true, true))
                if (hitInfo.collider.TryGetComponent<Mousable>(out var hitMousable)
                && hitMousable.enabled
                && hitMousable.PerformRaycastTests(hitInfo.point))
                {
                    CursorMousable = hitMousable;
                    CursorMousableHitInfo = hitInfo;
                    return;
                }

            CursorMousable = null;
            CursorMousableHitInfo = new RaycastHit
            {
                point = float.NaN.ToVector3(),
            };
        }
        static private void UpdateHighlights()
        {
            if (_previousCursorMousable != null
            && (CursorMousable == null || _previousCursorMousable != CursorMousable)
            && !_lockedMousablesByButton.ContainsValue(_previousCursorMousable))
                _previousCursorMousable.LoseHighlight();

            if (CursorMousable != null
            && (_previousCursorMousable == null || _previousCursorMousable != CursorMousable)
            && !_lockedMousablesByButton.ContainsValue(CursorMousable))
                CursorMousable.GainHighlight();
        }
        static private void UpdateButtonEvents(Button[] buttons)
        {
            foreach (var button in buttons)
            {
                // Press
                if (Input.GetMouseButtonDown((int)button)
                && !_lockedMousablesByButton.ContainsKey(button)
                && CursorMousable != null)
                {
                    CursorMousable.Press(button, CursorMousableHitInfo.point);
                    _lockedMousablesByButton.Add(button, CursorMousable);
                }

                // Find locked mousable
                if (!_lockedMousablesByButton.TryGetNonNull(button, out var lockedMousable))
                    continue;

                // Hold
                if (Input.GetMouseButton((int)button))
                    lockedMousable.Hold(button, CursorMousableHitInfo.point);

                // Release
                if (Input.GetMouseButtonUp((int)button))
                {
                    lockedMousable.Release(button, CursorMousableHitInfo.point);
                    if (lockedMousable != CursorMousable)
                        lockedMousable.LoseHighlight();
                    _lockedMousablesByButton.Remove(button);
                }
            }
        }

        // Events
        static public void InvokeEvents()
        {
            if (Input.mousePosition != _previousMousePosition)
                OnCameraMoved?.Invoke(_previousMousePosition, Input.mousePosition);
        }
        static public event System.Action<Vector2, Vector2> OnCameraMoved;

        // Play
        override public void PlayAwake()
        {
            base.PlayAwake();
            _previousMousePosition = Input.mousePosition;
            _previousCursorMousable = CursorMousable;
            CreateCursorTransform(_CursorPrefab, this);
            OnCameraMoved = null;

            _lockedMousablesByButton = new Dictionary<Button, Mousable>();
        }
        protected override void SubscribeToPlayEvents()
        {
            base.SubscribeToPlayEvents();
            Updatable.OnPlayUpdate += () =>
            {
                UpdateCursorMousable();
                UpdateHighlights();
                UpdateButtonEvents(_Buttons);
                CursorTransform.position = CameraManager.FirstActive.CursorToWorldPoint(_cursorDistance);
                InvokeEvents();

                _previousMousePosition = Input.mousePosition;
                _previousCursorMousable = CursorMousable;
            };
        }

        // Defines
        public enum Button
        {
            Left = 0,
            Right = 1,
            Middle = 2,
            Extra1 = 3,
            Extra2 = 4,
        }
    }
}