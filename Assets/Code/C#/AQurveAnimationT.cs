namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    abstract public class AQurveAnimation<T> : AQurveAnimation where T : struct
    {
        // Inspector  
        public T _To = default;

        // Privates
        protected T _from;

        // Publics
        abstract public T Value
        { get; }
        public void Start(T from, T to)
        {
            foreach (var anim in _mutualExclusives)
                anim.Stop();
            _from = from;
            _To = to;
            Start();
        }
    }
}