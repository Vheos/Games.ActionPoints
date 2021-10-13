namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Collections;
    using Tools.Extensions.UnityObjects;
    [DisallowMultipleComponent]
    public class CursorManager : AUpdatable
    {
        // Inspector
        public Button[] _Buttons;

        // Publics
        static public Plane CursorPlane
        { get; set; }
        static public Transform CursorTransform
        { get; private set; }
        static public AMousable CursorMousable
        { get; private set; }
        static public RaycastHit CursorMousableHitInfo
        { get; private set; }

        // Privates
        private AMousable _highlightedMousable;
        private List<Button> _heldButtons;
        private Vector3 _previousMousePosition;
        private bool MouseMoved
        => Input.mousePosition != _previousMousePosition;
        private void UpdateCursorMousable()
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
        private void UpdateHighlights(AMousable hitMousable)
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
        private void UpdateButtonEvents(RaycastHit hitInfo)
        {
            foreach (var button in _Buttons)
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
            CursorTransform = new GameObject(nameof(CursorTransform)).transform;
            CursorTransform.BecomeChildOf(this);

            //
            CursorPlane = new Plane(Vector3.back, Vector3.zero);
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
                UpdateButtonEvents(CursorMousableHitInfo);

            Ray cursorRay = CameraManager.FirstActive.CursorRay();
            if (CursorPlane.Raycast(cursorRay, out var distance))
                CursorTransform.position = cursorRay.GetPoint(distance);

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