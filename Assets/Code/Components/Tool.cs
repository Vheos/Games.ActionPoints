namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;

    [RequireComponent(typeof(SpriteRenderer))]
    public class Tool : ABaseComponent
    {
        // Inspector
        [SerializeField] protected Vector3 _LocalPositionOffset;
        [SerializeField] protected Vector3 _LocalRotationOffset;
        [SerializeField] protected ActionAnimation.Clip _Idle;
        [SerializeField] protected ActionAnimation.Clip _Unsheathe;
        [SerializeField] protected ActionAnimation.Clip _Sheathe;

        // Publics
        public ActionAnimation.Clip Idle
        => _Idle;
        public Character Character
        { get; private set; }
        public void GetEquippedBy(Character character)
        {
            Character = character;
            this.BecomeChildOf(character.HandTransform, true);
            transform.AnimateLocalPosition(this, _LocalPositionOffset, 0.5f);
            transform.AnimateLocalRotation(this, _LocalRotationOffset, 0.5f);
            this.GOActivate();
        }
        public void GetUnequipped()
        {
            Character = null;
            this.GODeactivate();
            this.Unparent();
        }
    }
}