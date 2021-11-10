namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;

    public class Woundable : ABaseComponent
    {
        // Inspector
        [SerializeField] [Range(0f, 100f)] protected float _BluntArmor = 0f;
        [SerializeField] [Range(0f, 100f)] protected float _SharpArmor = 0f;

        // Events   
        public Event<float, int> OnDamageReceived
        { get; } = new Event<float, int>();
        public Event<float, int> OnHealingReceived
        { get; } = new Event<float, int>();
        public Event<int, int> OnWoundsCountChanged
        { get; } = new Event<int, int>();

        // Publics
        public int WoundsCount
        { get; private set; }
        public float BluntArmor
        => _BluntArmor;
        public float SharpArmor
        => _SharpArmor;
        public float CalculateBluntDamage(float bluntDamage)
        => bluntDamage.Add(0 - _BluntArmor).ClampMin(0);
        public float CalculateSharpDamage(float sharpDamage)
        => sharpDamage.Mul(1 - _SharpArmor / 100f).ClampMin(0);
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
            float remainingChance = percentChance - sureHits;
            int rolledHits = remainingChance.RollPercent().To01();
            return sureHits + rolledHits;
        }
        private void RollForWounds(float chance)
        {
            int previousWoundsCount = WoundsCount;
            bool isHealing = chance < 0;
            chance.SetAbs();
            
            int unclampedWoundsDiff = GetRollHitsCount(chance) * isHealing.ToSign();
            WoundsCount = WoundsCount.Add(unclampedWoundsDiff).Clamp(0, _RawMaxPoints);
            int woundsDiff = previousWoundsCount.DistanceTo(WoundsCount);
            
            (isHealing ? OnHealingReceived : OnDamageReceived)?.Invoke(chance, woundsDiff);
            if (woundsDiff != 0)
                OnWoundsCountChanged?.Invoke(previousWoundsCount, WoundsCount);

            //_ui.PopupHandler.PopDamage(transform.position, totalDamage, totalWounds);
        }
    }
}