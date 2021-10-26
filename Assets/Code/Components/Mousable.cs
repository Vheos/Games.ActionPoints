namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public class Mousable : APlayable
    {
        // Events
        public event System.Action OnMouseGainHighlight;
        public event System.Action OnMouseLoseHighlight;
        public event System.Action<CursorManager.Button, Vector3> OnMousePress;
        public event System.Action<CursorManager.Button, Vector3> OnMouseHold;
        public event System.Action<CursorManager.Button, Vector3> OnMouseRelease;
        internal void MouseGainHighlight()
        => OnMouseGainHighlight?.Invoke();
        internal void MouseLoseHighlight()
        => OnMouseLoseHighlight?.Invoke();
        internal void MousePress(CursorManager.Button button, Vector3 position)
        => OnMousePress?.Invoke(button, position);
        internal void MouseHold(CursorManager.Button button, Vector3 position)
        => OnMouseHold?.Invoke(button, position);
        internal void MouseRelease(CursorManager.Button button, Vector3 position)
        => OnMouseRelease?.Invoke(button, position);

        // Publics
        virtual public bool RaycastTest(Vector3 location)
        => true;

        // Privates
        protected Collider _mousableCollider;
        private void AssignLayer()
        => gameObject.layer = LayerMask.NameToLayer(nameof(Mousable));


        // Privates


        // Play
        public override void PlayAwake()
        {
            base.PlayAwake();
            _mousableCollider = GetComponent<Collider>();
            AssignLayer();
        }
    }
}