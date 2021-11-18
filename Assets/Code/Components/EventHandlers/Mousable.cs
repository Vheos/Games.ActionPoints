namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.General;
    using Event = Tools.UnityCore.Event;
    using static CursorManager;
    using static UIManager;

    [DisallowMultipleComponent]
    public class Mousable : AEventSubscriber
    {
        // Events
        public Event OnGainHighlight
        { get; } = new Event();
        public Event OnLoseHighlight
        { get; } = new Event();
        public Event<MouseButton, Vector3> OnPress
        { get; } = new Event<MouseButton, Vector3>();
        public Event<MouseButton, Vector3> OnHold
        { get; } = new Event<MouseButton, Vector3>();
        public Event<MouseButton, bool> OnRelease
        { get; } = new Event<MouseButton, bool>();

        // Publics
        public Collider Trigger
        { get; private set; }
        public event Func<Vector3, bool> RaycastTests;
        public bool PerformRaycastTests(Vector3 position)
        {
            if (RaycastTests != null)
                foreach (Func<Vector3, bool> raycastTest in RaycastTests.GetInvocationList())
                    if (!raycastTest(position))
                        return false;
            return true;
        }

        // Privates
        private void AssignLayer()
        => gameObject.layer = LayerMask.NameToLayer(nameof(Mousable));
        protected void TryFitBoxColliderToMesh()
        {
            if (Trigger.TryAs<BoxCollider>(out var boxCollider)
            && TryGetComponent<MeshFilter>(out var meshFilter))
                boxCollider.size = meshFilter.mesh.bounds.size;
        }
        private void TryInvokeSelectableOnPress(Selectable selectable, MouseButton button)
        {
            ButtonFunction function = button.ToKeyCode().ToFunction();
            if (function != ButtonFunction.None)
                selectable.OnPress.Invoke(function);
        }
        private void TryInvokeSelectableOnHold(Selectable selectable, MouseButton button)
        {
            ButtonFunction function = button.ToKeyCode().ToFunction();
            if (function != ButtonFunction.None)
                selectable.OnHold.Invoke(function);
        }
        private void TryInvokeSelectableOnRelease(Selectable selectable, MouseButton button, bool isClick)
        {
            ButtonFunction function = button.ToKeyCode().ToFunction();
            if (function != ButtonFunction.None)
                selectable.OnRelease.Invoke(function, isClick);
        }

        // Play
        protected override void DefineAutoSubscriptions()
        {
            base.DefineAutoSubscriptions();
            if (TryGetComponent<Selectable>(out var selectable))
            {
                SubscribeTo(OnGainHighlight, selectable.OnGainHighlight.Invoke);
                SubscribeTo(OnLoseHighlight, selectable.OnLoseHighlight.Invoke);
                SubscribeTo(OnPress, (button, position) => TryInvokeSelectableOnPress(selectable, button));
                SubscribeTo(OnHold, (button, position) => TryInvokeSelectableOnHold(selectable, button));
                SubscribeTo(OnRelease, (button, isClick) => TryInvokeSelectableOnRelease(selectable, button, isClick));
            }
        }
        protected override void PlayAwake()
        {
            base.PlayAwake();
            Trigger = Get<Collider>();
            TryFitBoxColliderToMesh();
            AssignLayer();
        }
    }
}