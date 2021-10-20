namespace Vheos.Games.ActionPoints
{
    using UnityEngine;

    abstract public class AActionScript : ScriptableObject
    {
        abstract public void Invoke(Character user, Character target);
    }
}