namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.General;
    using Event = Tools.UnityCore.Event;

    [DisallowMultipleComponent]
    public class Mousable : ABaseComponent
    {
        // Events
        public Event OnGainHighlight
        { get; } = new Event();
        public Event OnLoseHighlight
        { get; } = new Event();
        public Event<CursorManager.Button, Vector3> OnPress
        { get; } = new Event<CursorManager.Button, Vector3>();
        public Event<CursorManager.Button, Vector3> OnHold
        { get; } = new Event<CursorManager.Button, Vector3>();
        public Event<CursorManager.Button, bool> OnRelease
        { get; } = new Event<CursorManager.Button, bool>();

        internal void GainHighlight()
        => OnGainHighlight?.Invoke();
        internal void LoseHighlight()
        => OnLoseHighlight?.Invoke();
        internal void Press(CursorManager.Button button, Vector3 position)
        => OnPress?.Invoke(button, position);
        internal void Hold(CursorManager.Button button, Vector3 position)
        => OnHold?.Invoke(button, position);
        internal void Release(CursorManager.Button button, bool isClick)
        => OnRelease?.Invoke(button, isClick);

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

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            Trigger = Get<Collider>();
            TryFitBoxColliderToMesh();
            AssignLayer();
        }
    }
}