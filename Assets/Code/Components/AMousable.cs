namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    [RequireComponent(typeof(Collider))]
    abstract public class AMousable : AUpdatable
    {
        // Virtuals
        virtual public void MouseGainHighlight()
        { }
        virtual public void MouseLoseHighlight()
        { }
        virtual public void MousePress(MouseManager.Button button, Vector3 location)
        { }
        virtual public void MouseRelease(MouseManager.Button button)
        { }
        virtual public bool RaycastTest(Vector3 location)
        => true;

        // Publics
        public bool RecieveMouseEvents
        { get; set; }

        // Privates
        private void AssignLayer()
        => gameObject.layer = LayerMask.NameToLayer(nameof(AMousable));

        // Mono
        public override void PlayAwake()
        {
            base.PlayAwake();
            AssignLayer();
            RecieveMouseEvents = true;
        }
    }
}