namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    abstract public class AAction : ScriptableObject
    {
        // Publics
        public Sprite Sprite = null;
        public void Invoke(Character character)
        {
            Debug.Log($"{character.name} + {GetType().Name}");
        }
    }
}