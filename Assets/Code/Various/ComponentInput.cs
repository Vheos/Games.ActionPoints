namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;

    abstract public class AComponentInput
    {
        protected bool _hasBeenSet;
        protected bool TestForWarnings(Type type)
        {
            if (_hasBeenSet)
                return WarningInputAlreadySet(type);
            return false;
        }
        protected bool WarningInputAlreadySet(Type type)
        {
            Debug.LogWarning($"InputAlreadySet:\ttrying to override an already defined component input of type {type.Name}\n" +
            $"Fallback:\treturn without changing anything");
            return true;
        }
    }

    public class ComponentInput<TReturn> : AComponentInput
    {
        // Publics
        public void Set(Func<TReturn> getFunction)
        {
            if (TestForWarnings(typeof(TReturn)))
                return;

            _getFunction = getFunction;
            _hasBeenSet = true;
        }
        public TReturn Value
        => _getFunction();
        public static implicit operator TReturn(ComponentInput<TReturn> t)
        => t._getFunction();

        // Privates   
        private Func<TReturn> _getFunction = () => default;
    }

    public class ComponentInput<T, TReturn> : AComponentInput
    {
        // Publics
        public void Set(Func<T, TReturn> getFunction)
        {
            if (TestForWarnings(typeof(TReturn)))
                return;

            _getFunction = getFunction;
            _hasBeenSet = true;
        }
        public TReturn Value(T arg1)
        => _getFunction(arg1);
        public TReturn this[T arg1]
        => _getFunction(arg1);

        // Privates   
        private Func<T, TReturn> _getFunction = (arg1) => default;
    }
}