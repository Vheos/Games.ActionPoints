namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    public struct ActionStats
    {
        public float BluntDamageDealt;
        public float SharpDamageDealt;
        public float RawDamageDealt;
        public float BluntDamageMitigated;
        public float SharpDamageMitigated;
        public int WoundsDealt;

        public float DamageHealed;
        public int WoundsHealed;


        public void AddDamageStats(float bluntSent, float bluntReceived, float sharpSent, float sharpReceived, float raw)
        {
            BluntDamageDealt += bluntReceived;
            BluntDamageMitigated += bluntSent - bluntReceived;
            SharpDamageDealt += sharpReceived;
            SharpDamageMitigated += sharpSent - sharpReceived;
            RawDamageDealt += raw;
        }
        public float TotalDamageDealt
        => BluntDamageDealt + SharpDamageDealt + RawDamageDealt;
    }
}