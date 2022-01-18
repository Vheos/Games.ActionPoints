/*
namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;
    using Tools.Extensions.Collections;
    using UnityEngine.InputSystem;

    [DisallowMultipleComponent]
    public class CursorManager : AComponentManager<CursorManager, UICursor>
    {
        // Inspector
        [SerializeField] protected MouseButton[] _Buttons;

        // Events
        static public AutoEvent<Vector2, Vector2> OnMoveCursor
        { get; private set; }
        static public AutoEvent<Mousable, Mousable> OnChangeCursorMousable
        { get; private set; }

        // Publics
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
        static private Vector3 _previousMousePosition;
        static private float _cursorDistance;
        static private Dictionary<MouseButton, Mousable> _lockedMousablesByButton;
        static private void UpdateCursorMousable()
        {
            Mousable previousCursorMousable = CursorMousable;
            CursorMousable = null;
            CursorMousableHitInfo = new RaycastHit
            {
                point = float.NaN.ToVector3(),
            };
            foreach (var hitInfo in NewUtility.RaycastFromCamera(CameraManager.FirstActive, LayerMask.GetMask(nameof(Mousable)), true, true))
                if (hitInfo.collider.TryGetComponent(out Mousable hitMousable)
                && hitMousable.enabled
                && hitMousable.PerformRaycastTests(hitInfo.point))
                {
                    CursorMousable = hitMousable;
                    CursorMousableHitInfo = hitInfo;
                    break;
                }
            if (CursorMousable != previousCursorMousable)
                OnChangeCursorMousable.Invoke(previousCursorMousable, CursorMousable);
        }
        static private void UpdateHighlights(Mousable from, Mousable to)
        {
            if (from != null
            && (to == null || from != to)
            && !_lockedMousablesByButton.ContainsValue(from))
                from.OnLoseHighlight.Invoke();

            if (to != null
            && (from == null || from != to)
            && !_lockedMousablesByButton.ContainsValue(to))
                to.OnGainHighlight.Invoke();
        }
        static private void InvokeMousableEvents(MouseButton[] buttons)
        {
            foreach (var button in buttons)
            {
                // Press
                if (Input.GetMouseButtonDown((int)button)
                && !_lockedMousablesByButton.ContainsKey(button)
                && CursorMousable != null)
                {
                    CursorMousable.OnPress.Invoke(button, CursorMousableHitInfo.point);
                    _lockedMousablesByButton.Add(button, CursorMousable);
                }

                // Find locked mousable
                if (!_lockedMousablesByButton.TryGetNonNull(button, out var lockedMousable))
                    continue;

                // Hold
                if (Input.GetMouseButton((int)button))
                    lockedMousable.OnHold.Invoke(button, CursorMousableHitInfo.point);

                // Release
                if (Input.GetMouseButtonUp((int)button))
                {
                    lockedMousable.OnRelease.Invoke(button, IsMousableUnderCursor(lockedMousable));
                    if (lockedMousable != CursorMousable)
                        lockedMousable.OnLoseHighlight.Invoke();
                    _lockedMousablesByButton.Remove(button);
                }
            }
        }
        static private void OnUpdate()
        {
            TryInvokeOnMoveCursor();
            UpdateCursorMousable();
            InvokeMousableEvents(_instance._Buttons);
        }
        static private void TryInvokeOnMoveCursor()
        {
            if (Input.mousePosition != _previousMousePosition)
                OnMoveCursor.Invoke(_previousMousePosition, Input.mousePosition);

            _previousMousePosition = Input.mousePosition;
        }
        static private bool IsMousableUnderCursor(Mousable mousable)
        => mousable.Trigger.CursorRaycast(CameraManager.FirstActive, out var hitInfo)
        && mousable.PerformRaycastTests(hitInfo.point);

        // Initializers
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static private void StaticInitialize()
        {
            OnMoveCursor = new AutoEvent<Vector2, Vector2>();
            OnChangeCursorMousable = new AutoEvent<Mousable, Mousable>();
        }

        // Play
        protected override void DefineAutoSubscriptions()
        {
            base.DefineAutoSubscriptions();
            SubscribeAuto(Get<Updatable>().OnUpdate, OnUpdate);
            SubscribeAuto(OnChangeCursorMousable, UpdateHighlights);
        }
        override protected void PlayAwake()
        {
            base.PlayAwake();
            _cursorDistance = 0f;
            _lockedMousablesByButton = new Dictionary<MouseButton, Mousable>();
        }
    }
}
*/