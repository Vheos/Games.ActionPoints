namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;

    [RequireComponent(typeof(SpriteRenderer))]
    public class Tool : ABaseComponent
    {
        // Inspector
        [SerializeField] protected Vector3 _LocalPositionOffset = Vector3.zero;
        [SerializeField] protected Vector3 _LocalRotationOffset = Vector3.zero;
        [SerializeField] protected ActionAnimation.Clip _Idle = new ActionAnimation.Clip();
        [SerializeField] protected ActionAnimation.Clip[] _Unsheathe = new ActionAnimation.Clip[1];  
        [SerializeField] protected ActionAnimation.Clip[] _Sheathe = new ActionAnimation.Clip[1];

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