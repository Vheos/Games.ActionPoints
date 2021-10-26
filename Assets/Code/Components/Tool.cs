namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;

    [RequireComponent(typeof(SpriteRenderer))]
    public class Tool : APlayable
    {
        // Inspector
        public ActionAnimation.StateData _Idle;

        // Publics
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
        // Privates


        // Mono

    }
}