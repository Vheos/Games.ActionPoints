namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    [DisallowMultipleComponent]
    public class MouseManager : AUpdatable
    {
        // Inspector
        [SerializeField] protected Button[] __Buttons;

        // Privates
        private AMousable _highlightedMousable;
        private List<Button> _heldButtons;
        private Vector3 _previousMousePosition;
        private bool MouseMoved
        =>  Input.mousePosition != _previousMousePosition;
        
        private bool TryFindMousable(out AMousable hitMousable, out RaycastHit hitInfo)
        {
            hitMousable = null;
            hitInfo = default;

            if (CameraManager.CursorCamera.TryNonNull(out var activeCamera)
            && Physics.Raycast(activeCamera.CursorRay(), out hitInfo, float.PositiveInfinity, LayerMask.GetMask(nameof(AMousable)), QueryTriggerInteraction.Collide))
                hitInfo.collider.TryGetComponent(out hitMousable);

            return hitMousable != null;
        }
        private void UpdateHighlights(AMousable hitMousable)
        {
            // clickable -> void
            if (_highlightedMousable != null && (hitMousable == null || _highlightedMousable != hitMousable))
            {
                _highlightedMousable.MouseLoseHighlight();
                _highlightedMousable = null;
                _heldButtons.Clear();
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
            foreach (var button in __Buttons)
                // press highlighted clickable
                if (Input.GetMouseButtonDown((int)button) && !_heldButtons.Contains(button))
                {
                    _heldButtons.Add(button);
                    _highlightedMousable.MousePress(button, hitInfo.point);
                }
                // release highlighted clickable
                else if (Input.GetMouseButtonUp((int)button) && _heldButtons.Contains(button))
                {
                    _highlightedMousable.MouseRelease(button);
                    _heldButtons.Remove(button);
                }
        }

        // Mono
        override public void PlayAwake()
        {
            base.PlayAwake();
            _previousMousePosition = Input.mousePosition;
            _heldButtons = new List<Button>();
        }
        override public void PlayUpdate()
        {
            base.PlayUpdate();
            if (MouseMoved)
                CameraManager.SetDirtyCursorCamera();
           
            TryFindMousable(out var hitMousable, out var hitInfo);
            if (_highlightedMousable == null || !_highlightedMousable.IsHighlightLocked)
                UpdateHighlights(hitMousable);
            if (_highlightedMousable != null)
                UpdateButtonEvents(hitInfo);

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