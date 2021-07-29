namespace Vheos.Games.ActionPoints
{
    using System;
    public class Cached<T>
    {
        // Publics
        public T Cache
        {
            get
            {
                if (!_hasBeenCached)
                {
                    _cachedValue = _cachingFunction();
                    _hasBeenCached = true;
                }
                return _cachedValue;
            }
        }

        // Privates
        protected Func<T> _cachingFunction;
        protected T _cachedValue;
        protected bool _hasBeenCached;

        // Constructors
        public Cached(Func<T> cachingFunction)
        {
            _cachingFunction = cachingFunction;
            _hasBeenCached = false;
        }

        // Methods
        public void Recache()
        => _hasBeenCached = false; 

        // Operators
        public static implicit operator T(Cached<T> cached)
        => cached.Cache; 
    }
}