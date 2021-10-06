namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    abstract public class AQurveAnimation : Timer
    {
        // Inspector  
        public float _Duration = 0.5f;

        // Privates
        protected float QurveValue
        => Qurve.ValueAt(Progress);
        protected List<AQurveAnimation> _mutualExclusives;

        // Publics
        public void SetMutuallyExclusiveWith(AQurveAnimation anim)
        {
            _mutualExclusives.Add(anim);
            anim._mutualExclusives.Add(this);
        }

        // Overrides
        public override float Duration
        => _Duration;
        public override bool SkipFrameZero
        => true;

        // Constructors
        protected AQurveAnimation()
        => _mutualExclusives = new List<AQurveAnimation>();
    }
}