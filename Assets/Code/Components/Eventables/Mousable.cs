namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using System;
    using Tools.Extensions.General;

    [DisallowMultipleComponent]
    public class Mousable : APlayable
    {
        // Events
        public event System.Action OnGainHighlight;
        public event System.Action OnLoseHighlight;
        public event System.Action<CursorManager.Button, Vector3> OnPress;
        public event System.Action<CursorManager.Button, Vector3> OnHold;
        public event System.Action<CursorManager.Button, Vector3> OnRelease;
        internal void GainHighlight()
        => OnGainHighlight?.Invoke();
        internal void LoseHighlight()
        => OnLoseHighlight?.Invoke();
        internal void Press(CursorManager.Button button, Vector3 position)
        => OnPress?.Invoke(button, position);
        internal void Hold(CursorManager.Button button, Vector3 position)
        => OnHold?.Invoke(button, position);
        internal void Release(CursorManager.Button button, Vector3 position)
        => OnRelease?.Invoke(button, position);

        // Publics
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
        private Collider _trigger;
        private void AssignLayer()
        => gameObject.layer = LayerMask.NameToLayer(nameof(Mousable));
        protected void TryFitBoxColliderToMesh()
        {
            if (_trigger.TryAs<BoxCollider>(out var boxCollider)
            && TryGetComponent<MeshFilter>(out var meshFilter))
                boxCollider.size = meshFilter.mesh.bounds.size;
        }

        // Play
        public override void PlayAwake()
        {
            base.PlayAwake();
            _trigger = GetComponent<Collider>();
            TryFitBoxColliderToMesh();
            AssignLayer();
        }
    }
}