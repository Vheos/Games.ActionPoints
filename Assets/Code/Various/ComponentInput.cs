namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;

    public class ComponentInput<T>
    {
        // Publics
        public void Set(Func<T> getFunction)
        {
            if (_hasBeenSet)
            {
                WarningInputAlreadySet(typeof(T));
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
        private void WarningInputAlreadySet(Type type)
        => Debug.LogWarning($"InputAlreadySet:\ttrying to override an already defined component input of type {type.Name}\n" +
        $"Fallback: return without changing anything");

        // Initializers
        public ComponentInput()
        => Reset();

        // Operators
        public static implicit operator T(ComponentInput<T> t)
        => t._getFunction();
    }
}