namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.General;
        using static CursorManager;
    using static UIManager;

    [DisallowMultipleComponent]
    public class Mousable : AAutoSubscriber
    {
        // Events
        public AutoEvent OnGainHighlight
        { get; } = new AutoEvent();
        public AutoEvent OnLoseHighlight
        { get; } = new AutoEvent();
        public AutoEvent<MouseButton, Vector3> OnPress
        { get; } = new AutoEvent<MouseButton, Vector3>();
        public AutoEvent<MouseButton, Vector3> OnHold
        { get; } = new AutoEvent<MouseButton, Vector3>();
        public AutoEvent<MouseButton, bool> OnRelease
        { get; } = new AutoEvent<MouseButton, bool>();

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
            && TryGet<MeshFilter>(out var meshFilter))
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
            if (TryGet<Selectable>(out var selectable))
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
#if CACHED_COMPONENTS
        protected override void DefineCachedComponents()
        {
            base.DefineCachedComponents();
            TryAddToCache<Collider>();
        }
#endif
    }
}