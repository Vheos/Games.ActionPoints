namespace Vheos.Games.ActionPoints.ActionScripts
{
    using UnityEngine;
    using Tools.Extensions.Math;

    [CreateAssetMenu(fileName = nameof(DealDamage), menuName = CONTEXT_MENU_PATH + nameof(DealDamage), order = 2)]
    public class DealDamage : AActionEffect
    {
        // Overrides
        protected override int RequiredValuesCount
        => 3;
        override public void Invoke(Character user, Character target, params float[] values)
        {
            // Params
            float blunt = values[0];
            float sharp = values[1];
            float raw = values[2];

            // Execute
            target.TakeTotalDamage(target.CalculateTotalDamage(blunt, sharp, raw));
        }
    }
}