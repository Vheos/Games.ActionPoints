namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    public class Action : ScriptableObject
    {
        // Inspector
        public Sprite Sprite = null;
        [Range(0, 5)] public int ActionPointsCost = 1;
        [Range(0, 5)] public int FocusPointsCost = 1;
        public bool IsTargeted = false;

        // Publics
        public bool CanBeUsed(Character character)
        {
            return character.ActionProgress >= 0
                && character.ActionPointsCount >= ActionPointsCost
                && character.FocusPointsCount >= FocusPointsCost;
        }
        public void Use(Character user, Character target)
        {
            user.ChangeActionPoints(-ActionPointsCost);
            user.ChangeFocusPoints(-FocusPointsCost);
            Debug.Log($"{user.name} used {name} on {target.name}");
        }
    }
}