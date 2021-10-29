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

        // Publics
        public ActionAnimation.Clip Idle
        => _Idle;
        public Character Character
        { get; private set; }
        public void GetEquippedBy(Character character)
        {
            Character = character;
            this.BecomeChildOf(character.HandTransform);
            transform.localPosition = _LocalPositionOffset;
            transform.localRotation = Quaternion.Euler(_LocalRotationOffset);
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