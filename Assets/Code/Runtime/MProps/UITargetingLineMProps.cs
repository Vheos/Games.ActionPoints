namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    sealed public class UITargetingLineMProps : AAutoMProps
    {
        // Publics
        public float TilingX
        {
            get => GetFloat(nameof(TilingX));
            set => SetFloat(nameof(TilingX), value);
        }
    }
}