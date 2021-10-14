namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Collections;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;

    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-1)]
    public class CursorManager : AUpdatable
    {
        // Inspector
        public Button[] _Buttons;
        public GameObject _CursorPrefab;

        // Publics
        static public Transform CursorTransform
        { get; private set; }
        static public AMousable CursorMousable
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
        static private AMousable _highlightedMousable;
        static private List<Button> _heldButtons;
        static private Vector3 _previousMousePosition;
        static private float _cursorDistance;
        static private bool MouseMoved
        => Input.mousePosition != _previousMousePosition;
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
            if (CameraManager.CursorCamera.TryNonNull(out var activeCamera))
                foreach (var hitInfo in Physics.RaycastAll(activeCamera.CursorRay(), float.PositiveInfinity, LayerMask.GetMask(nameof(AMousable)), QueryTriggerInteraction.Collide))
                    if (hitInfo.collider.TryGetComponent<AMousable>(out var hitMousable)
                    && hitMousable.enabled
                    && hitMousable.RecieveMouseEvents
                    && hitMousable.RaycastTest(hitInfo.point))
                    {
                        CursorMousable = hitMousable;
                        CursorMousableHitInfo = hitInfo;
                        return;
                    }

            CursorMousable = null;
            CursorMousableHitInfo = default;
        }
        static private void UpdateHighlights(AMousable hitMousable)
        {
            // clickable -> void
            if (_highlightedMousable != null && (hitMousable == null || _highlightedMousable != hitMousable))
            {
                _highlightedMousable.MouseLoseHighlight();
                _highlightedMousable = null;
            }
            // void -> clickable
            if (hitMousable != null && (_highlightedMousable == null || _highlightedMousable != hitMousable))
            {
                _highlightedMousable = hitMousable;
                _highlightedMousable.MouseGainHighlight();
            }
        }
        static private void UpdateButtonEvents(IEnumerable<Button> buttons, RaycastHit hitInfo)
        {
            foreach (var button in buttons)
            {   // Press highlighted clickable
                if (Input.GetMouseButtonDown((int)button) && !_heldButtons.Contains(button))
                {
                    _heldButtons.Add(button);
                    _highlightedMousable.MousePress(button, hitInfo.point);
                }
                // Release highlighted clickable
                else if (Input.GetMouseButtonUp((int)button) && _heldButtons.Contains(button))
                {
                    _highlightedMousable.MouseRelease(button, hitInfo.point);
                    _heldButtons.Remove(button);
                }
                // Hold
                if (Input.GetMouseButton((int)button))
                {
                    _highlightedMousable.MouseHold(button, hitInfo.point);
                }
            }
        }

        // Mono
        override public void PlayAwake()
        {
            base.PlayAwake();
            _previousMousePosition = Input.mousePosition;
            _heldButtons = new List<Button>();
            CreateCursorTransform(_CursorPrefab, this);
        }
        override public void PlayUpdate()
        {
            base.PlayUpdate();
            if (MouseMoved)
                CameraManager.SetDirtyCursorCamera();

            UpdateCursorMousable();
            if (_highlightedMousable == null || _heldButtons.IsEmpty())
                UpdateHighlights(CursorMousable);
            if (_highlightedMousable != null)
                UpdateButtonEvents(_Buttons, CursorMousableHitInfo);

            CursorTransform.position = CameraManager.FirstActive.CursorToWorldPoint(_cursorDistance);

            _previousMousePosition = Input.mousePosition;
        }

        // Enum
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