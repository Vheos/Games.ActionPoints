namespace Vheos.Games.Core
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [DisallowMultipleComponent]
    sealed public class Highlightable : AUsableByMany<Highlightable, Highlighter>
    {
        // Events
        public AutoEvent<Highlightable, Highlighter> OnGainHighlight
        => OnStartBeingUsed;
        public AutoEvent<Highlightable, Highlighter> OnLoseHighlight
        => OnStopBeingUsed;

        // Publics
        public bool IsHighlighted
        => IsBeingUsed;
        public bool IsHighlightedByMany
        => IsBeingUsedByMany;
        public bool IsHighlightedBy(Highlighter highlighter)
        => IsBeingUsedBy(highlighter);
        public void GainHighlightFrom(Highlighter highlighter)
        => StartBeingUsedBy(highlighter);
        public void LoseHighlightFrom(Highlighter highlighter)
        => StopBeingUsedBy(highlighter);
    }
}