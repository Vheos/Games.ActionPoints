namespace Vheos.Games.ActionPoints.ActionScripts
{
    using UnityEngine;
    using Tools.Extensions.Math;

    [CreateAssetMenu(fileName = nameof(InflictWounds), menuName = "ActionScripts/" + nameof(InflictWounds))]
    public class InflictWounds : AActionScript
    {
        // Overrides
        protected override int RequiredValuesCount
        => 1;
        override public void Invoke(Character user, Character target, params float[] values)
        {
            // Params
            int amount = values[0].Round();

            // Execute
            target.ChangeWoundsCount(amount);
        }
    }
}