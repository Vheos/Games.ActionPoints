namespace Vheos.Games.ActionPoints.ActionScripts
{
    using UnityEngine;

    [CreateAssetMenu(fileName = nameof(HealWounds), menuName = "ActionScripts/" + nameof(HealWounds))]
    public class HealWounds : AActionScript
    {
        // Inspector
        [Range(0, 10)] public int _Amount;

        // Overrides
        override public void Invoke(Character user, Character target)
        {
            target.ChangeWoundsCount(-_Amount);
        }
    }
}