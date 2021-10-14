namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.Extensions.Math;
    abstract public class AQurveAnimation : Timer
    {
        // Inspector  
        public float _Duration = 0.5f;
        public bool _IsBoomerang = false;

        // Publics
        public float QurveValue
        {
            get
            {
                float qurveValue = Qurve.ValueAt(Progress);
                if (_IsBoomerang)
                    qurveValue = qurveValue.Sub(0.5f).Abs().Neg().Add(0.5f);
                return qurveValue;
            }
        }
        public void SetPriorityAbove(AQurveAnimation anim)
        {
            _lowerAnims.Add(anim);
            anim._higherAnims.Add(this);
        }
        public void SetPriorityBelow(AQurveAnimation anim)
        {
            _higherAnims.Add(anim);
            anim._lowerAnims.Add(this);
        }
        public void SetInterruptingWith(AQurveAnimation anim)
        {
            _lowerAnims.Add(anim);
            anim._lowerAnims.Add(this);
        }
        public void SetWaitingWith(AQurveAnimation anim)
        {
            _higherAnims.Add(anim);
            anim._higherAnims.Add(this);
        }

        // Privates
        protected List<AQurveAnimation> _higherAnims;
        protected List<AQurveAnimation> _lowerAnims;

        // Overrides
        public override float Duration
        => _Duration;
        public override bool SkipFrameZero
        => true;

        // Constructors
        protected AQurveAnimation()
        {
            _lowerAnims = new List<AQurveAnimation>();
            _higherAnims = new List<AQurveAnimation>();
        }
    }
}