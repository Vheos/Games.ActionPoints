namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;
    using Tools.Extensions.General;

    [DisallowMultipleComponent]
    sealed public class Woundable : ABaseComponent
    {
        // Events   
        public readonly AutoEvent<float, bool> OnReceiveDamage = new();
        public readonly AutoEvent<float, bool> OnReceiveHealing = new();
        public readonly AutoEvent<int, int> OnChangeWounds = new();
        public readonly AutoEvent OnDie = new();

        // Inputs
        public readonly Getter<int> MaxWounds = new();
        public readonly Getter<float> BluntArmor = new();
        public readonly Getter<float> SharpArmor = new();

        // Publics
        public int Wounds
        {
            get => _wounds;
            set
            {
                if (value == _wounds)
                    return;

                int previousWounds = _wounds;
                _wounds = value;

                OnChangeWounds.Invoke(previousWounds, Wounds);
                CheckIsDead();
            }
        }
        public bool IsDead
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
        private int _wounds;
        private int GetRollHitsCount(float percentChance)
        {
            int sureHits = percentChance.Div(100f).RoundDown();
            float remainingChance = percentChance - sureHits * 100f;
            int rolledHits = remainingChance.RollPercent().To01();
            return sureHits + rolledHits;
        }
        private void RollForWounds(float chance)
        {
            int previousWounds = Wounds;
            bool isDamage = chance >= 0;
            chance.SetAbs();

            int unclampedWoundsDiff = GetRollHitsCount(chance) * isDamage.ToSign();
            int newWounds = Wounds.Add(unclampedWoundsDiff).Clamp(0, MaxWounds);

            (isDamage ? OnReceiveDamage : OnReceiveHealing).Invoke(chance, newWounds != previousWounds);
            Wounds = newWounds;
        }
        private void CheckIsDead()
        {
            if (Wounds < MaxWounds)
                return;

            IsDead = true;
            OnDie.Invoke();
        }
    }
}