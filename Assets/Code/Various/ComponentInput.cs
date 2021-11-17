namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;

    public class ComponentInput<T>
    {
        // Publics
        public void Set(Func<T> getFunction)
        {
            if (_hasBeenSet)
            {
                WarningInputAlreadySet();
                return;
            }

            _getFunction = getFunction;
            _hasBeenSet = true;
        }
        public void Reset()
        {
            _getFunction = Default;
            _hasBeenSet = false;
        }
        public T Value
        => _getFunction();

        // Privates   
        static private Func<T> Default
        => () => default;
        private Func<T> _getFunction = Default;
        private bool _hasBeenSet;
        private void WarningInputAlreadySet()
        => Debug.LogWarning($"InputAlreadySet:\ttrying to override an already defined component input\n" +
        $"Fallback: return without changing anyting");

        // Initializers
        public ComponentInput()
        => Reset();

        // Operators
        public static implicit operator T(ComponentInput<T> t)
        => t._getFunction();
    }
}