namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using System;

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
        public Collider Trigger
        { get; private set; }

        // Privates
        private void AssignLayer()
        => gameObject.layer = LayerMask.NameToLayer(nameof(Mousable));

        // Play
        public override void PlayAwake()
        {
            base.PlayAwake();
            Trigger = GetComponent<Collider>();
            AssignLayer();
        }
    }
}