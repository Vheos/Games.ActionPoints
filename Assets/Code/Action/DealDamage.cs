namespace Vheos.Games.ActionPoints.ActionScripts
{
    using UnityEngine;
    using Tools.Extensions.Math;

    [CreateAssetMenu(fileName = nameof(DealDamage), menuName = "ActionScripts/" + nameof(DealDamage))]
    public class DealDamage : AActionScript
    {
        // Overrides
        protected override int RequiredValuesCount
        => 3;
        override public void Invoke(Character user, Character target, params float[] values)
        {
            // Params
            int blunt = values[0].Round();
            int sharp = values[1].Round();
            int raw = values[2].Round();

            // Execute
            target.ChangeWoundsCount(blunt + sharp + raw);
        }
    }
}