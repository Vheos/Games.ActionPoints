namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    public class Action : ScriptableObject
    {
        // Publics
        public Sprite Sprite = null;
        [Range(0, 5)] public int ActionPointsCost = 1;
        [Range(0, 5)] public int FocusPointsCost = 1;
        public bool IsTargeted = false;
        public void Invoke(Character character)
        {
            Debug.Log($"{character.name} + {GetType().Name}");
        }

        public bool CanBeUsed(Character character)
        {
            return true;
        }
    }
}