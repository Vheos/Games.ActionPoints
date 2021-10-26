namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;

    [RequireComponent(typeof(SpriteRenderer))]
    public class Tool : ABaseComponent
    {
        // Inspector
        [SerializeField] protected ActionAnimation.StateData _Idle;

        // Publics
        public ActionAnimation.StateData Idle
        => _Idle;
        public Character Character
        { get; private set; }
        public void GetEquippedBy(Character character)
        {
            Character = character;
            this.BecomeChildOf(character.HandTransform);
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