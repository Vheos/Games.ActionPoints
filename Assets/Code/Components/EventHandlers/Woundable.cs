namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;

    public class Woundable : ABaseComponent
    {
        // Events   
        public Event<float, int> OnDamageReceived
        { get; } = new Event<float, int>();
        public Event<float, int> OnHealingReceived
        { get; } = new Event<float, int>();
        public Event<int, int> OnWoundsCountChanged
        { get; } = new Event<int, int>();

        // Inputs
        public ComponentInput<int> MaxWounds
        { get; } = new ComponentInput<int>();
        public ComponentInput<float> BluntArmor
        { get; } = new ComponentInput<float>();
        public ComponentInput<float> SharpArmor
        { get; } = new ComponentInput<float>();

        // Publics
        public int WoundsCount
        { get; private set; }
        public float CalculateBluntDamage(float bluntDamage)
        => bluntDamage.Add(0 - BluntArmor).ClampMin(0);
        public float CalculateSharpDamage(float sharpDamage)
        => sharpDamage.Mul(1 - SharpArmor / 100f).ClampMin(0);
        public float CalculateRawDamage(float rawDamage)
        => rawDamage.ClampMin(0);
        public float CalculateTotalDamage(float blunt, float sharp, float raw)
        => CalculateBluntDamage(blunt) + CalculateSharpDamage(sharp) + CalculateRawDamage(raw);
        public void ReceiveDamage(float blunt, float sharp, float raw)
        => RollForWounds(CalculateTotalDamage(blunt, sharp, raw));
        public float CalculateHealing(float healing)
        => healing.ClampMin(0);
        public void ReceiveHealing(float healing)
        => RollForWounds(CalculateHealing(healing).Neg());

        // Privates
        private int GetRollHitsCount(float percentChance)
        {
            int sureHits = percentChance.Div(100f).RoundDown();
            float remainingChance = percentChance - sureHits * 100f;
            int rolledHits = remainingChance.RollPercent().To01();
            return sureHits + rolledHits;
        }
        private void RollForWounds(float chance)
        {
            int previousWoundsCount = WoundsCount;
            bool isDamage = chance >= 0;
            chance.SetAbs();

            int unclampedWoundsDiff = GetRollHitsCount(chance) * isDamage.ToSign();
            WoundsCount = WoundsCount.Add(unclampedWoundsDiff).Clamp(0, MaxWounds);
            int woundsDiff = previousWoundsCount.DistanceTo(WoundsCount);

            (isDamage ? OnDamageReceived : OnHealingReceived)?.Invoke(chance, woundsDiff);
            if (woundsDiff != 0)
                OnWoundsCountChanged?.Invoke(previousWoundsCount, WoundsCount);
        }
    }
}