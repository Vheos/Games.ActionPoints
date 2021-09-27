namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    abstract public class AQurveAnimation<T> : Timer
    {
        // Inspector  
        [SerializeField] protected T __To;
        [SerializeField] [Range(0f, 1f)] protected float __HalfTime;
        [SerializeField] [Range(0f, 10f)] protected float __Duration;

        // Privates
        protected T _from;
        protected float QurveValue
        => Qurve.ValueAt(Progress, __HalfTime);

        // Publics
        public void Start(T from, T to)
        {
            _from = from;
            __To = to;
            Start();

        }
        abstract public T Value
        { get; }

        // Overrides
        public override float Duration
        => __Duration;
        public override bool SkipFrameZero
        => true;
    }
}