namespace Vheos.Games.Core
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [DisallowMultipleComponent]
    sealed public class Highlightable : ABaseComponent
    {
        // Events
        public readonly AutoEvent<bool> OnGainHighlight = new();
        public readonly AutoEvent<bool> OnLoseHighlight = new();

        // Publics
        public bool IsHighlighted
        => _highlightersCount > 0;
        public void GainHighlight()
        {
            _highlightersCount++;
            OnGainHighlight.Invoke(_highlightersCount == 1);   // is first
        }
        public void LoseHighlight()
        {
            _highlightersCount--;
            OnLoseHighlight.Invoke(_highlightersCount == 0);   // is last
        }
        public void ClearAllHighlights()
        {
            for (int i = 0; i < _highlightersCount; i++)
            {
                _highlightersCount--;
                OnLoseHighlight.Invoke(_highlightersCount == 0);
            }
        }

        // Privates
        private int _highlightersCount;

        // Play
        protected override void PlayDisable()
        {
            base.PlayDisable();
            ClearAllHighlights();
        }
    }
}