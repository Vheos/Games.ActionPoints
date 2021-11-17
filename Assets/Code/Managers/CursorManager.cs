namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;
    using Tools.Extensions.General;

    [DisallowMultipleComponent]
    sealed public class CursorManager : AManager<CursorManager>
    {
        // Inspector
        [SerializeField] private Button[] _Buttons;
        [SerializeField] private GameObject _CursorPrefab;

        // Events
        static public Event<Vector2, Vector2> OnCursorMoved
        { get; private set; }
        static public Event<Mousable, Mousable> OnCursorMousableChanged
        { get; private set; }

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
        static private float _cursorDistance;
        static private Vector3 _previousMousePosition;
        static private Mousable _previousCursorMousable;
        static private Dictionary<Button, Mousable> _lockedMousablesByButton;
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
                    bool isClick = lockedMousable.Trigger.IsUnderCursor(CameraManager.FirstActive);
                    lockedMousable.Release(button, isClick);
                    if (lockedMousable != CursorMousable)
                        lockedMousable.LoseHighlight();
                    _lockedMousablesByButton.Remove(button);
                }
            }
        }
        static private void OnUpdate()
        {

            UpdateCursorMousable();
            UpdateHighlights();
            UpdateButtonEvents(_instance._Buttons);
            CursorTransform.position = CameraManager.FirstActive.CursorToWorldPoint(_cursorDistance);
            TryInvokeEvents();

            _previousMousePosition = Input.mousePosition;
            _previousCursorMousable = CursorMousable;

        }
        static private void TryInvokeEvents()
        {
            if (Input.mousePosition != _previousMousePosition)
                OnCursorMoved?.Invoke(_previousMousePosition, Input.mousePosition);

            if (CursorMousable != _previousCursorMousable)
                OnCursorMousableChanged?.Invoke(_previousCursorMousable, CursorMousable);
        }

        // Initializers
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static private void StaticInitialize()
        {
            OnCursorMoved = new Event<Vector2, Vector2>();
            OnCursorMousableChanged = new Event<Mousable, Mousable>();
        }

        // Play
        protected override void AutoSubscribeToEvents()
        {
            base.AutoSubscribeToEvents();
            SubscribeTo(Get<Updatable>().OnUpdated, OnUpdate);
        }
        override protected void PlayAwake()
        {
            base.PlayAwake();
            _cursorDistance = 0f;
            _previousMousePosition = Input.mousePosition;
            _previousCursorMousable = CursorMousable;
            _lockedMousablesByButton = new Dictionary<Button, Mousable>();
            CreateCursorTransform(_CursorPrefab, this);
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