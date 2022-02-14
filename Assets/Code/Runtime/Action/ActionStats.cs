namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    public class ActionStats
    {
        // Stats
        public float BluntDamageDealt;
        public float SharpDamageDealt;
        public float PureDamageDealt;
        public float BluntDamageMitigated;
        public float SharpDamageMitigated;
        public int WoundsDealt;
        public float DamageHealed;
        public int WoundsHealed;

        // Helpers
        public void AddDamageStats(float bluntSent, float bluntReceived, float sharpSent, float sharpReceived, float pure)
        {
            BluntDamageDealt += bluntReceived;
            BluntDamageMitigated += bluntSent - bluntReceived;
            SharpDamageDealt += sharpReceived;
            SharpDamageMitigated += sharpSent - sharpReceived;
            PureDamageDealt += pure;
        }
        public float TotalDamageDealt
        => BluntDamageDealt + SharpDamageDealt + PureDamageDealt;
    }
}