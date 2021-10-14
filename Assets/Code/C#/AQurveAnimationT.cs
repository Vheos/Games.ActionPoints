namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    abstract public class AQurveAnimation<T> : AQurveAnimation where T : struct
    {
        // Inspector  
        public T _From = new T();
        public T _To = new T();

        // Publics
        abstract public T Value
        { get; }
        public override void Start()
        {
            if (_higherAnims.Any(a => a.IsActive))
                return;

            foreach (var anim in _lowerAnims)
                anim.Stop();
            base.Start();
        }
        public void Start(T from)
        {
            _From = from;
            Start();
        }
        public void Start(T from, T to)
        {
            _From = from;
            _To = to;
            Start();
        }
        public void Start(T from, T to, float duration)
        {
            _From = from;
            _To = to;
            _Duration = duration;
            Start();
        }
    }
}