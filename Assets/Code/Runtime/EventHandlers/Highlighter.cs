namespace Vheos.Games.Core
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class Highlighter : AUserOfMany<Highlighter, Highlightable>
    {
        // Events
        public AutoEvent<Highlighter, Highlightable> OnChangeHighlightable
        => OnAddUsable;

        // Publics
        public IReadOnlyCollection<Highlightable> Highlightables
        => _usables; 
        public bool IsHighlightingAny
        => IsUsingAny;
        public bool IsHighlighting(Highlightable highlightable)
        => IsUsing(highlightable);
        public bool IsHighlighting<T>() where T : Component
        => IsUsing<T>();
        public bool TryAddHighlightable(Highlightable highlightable)
        => TryAddUsable(highlightable);
        public bool TryRemoveHighlightable(Highlightable highlightable)
        => TryRemoveUsable(highlightable);
    }
}