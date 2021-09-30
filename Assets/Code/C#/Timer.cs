namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    public class Timer
    {
        // Publics
        static public Timer Create(float duration, bool skipFrameZero = false)
        => new Timer
        {
            _duration = duration,
            _skipFrameZero = skipFrameZero,
            _hasBeenStarted = false,
        };
        static public Timer CreateAndStart(float duration, bool skipFrameZero = false)
        {
            Timer newTimer = Create(duration, skipFrameZero);
            newTimer.Start();
            return newTimer;
        }
        virtual public float Duration
        => _duration;
        virtual public bool SkipFrameZero
        => _skipFrameZero;
        public void Start()
        {
            _startTime = Time.time;
            if (SkipFrameZero)
                _startTime -= Time.deltaTime;
            _hasBeenStarted = true;
        }
        public void Stop()
        => _hasBeenStarted = false;
        public float Elapsed
        => Time.time - _startTime;
        public float Progress
        => Elapsed / Duration;
        public bool IsActive
        => _hasBeenStarted && Elapsed < Duration;

        // Privates
        private float _duration;
        private bool _skipFrameZero;
        private bool _hasBeenStarted;
        private float _startTime;
    }
}



/*
namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    public class Timer
    {
        // Publics
        static public Timer Create(float duration, bool skipFrameZero = false)
        => new Timer
        {
            _duration = duration,
            _skipFrameZero = skipFrameZero,
            _hasBeenStarted = false,
        };
        static public Timer CreateAndStart(float duration, bool skipFrameZero = false)
        {
            Timer newTimer = Create(duration, skipFrameZero);
            newTimer.Start();
            return newTimer;
        }
        virtual public float Duration
        => _duration;
        virtual public bool SkipFrameZero
        => _skipFrameZero;
        public void Start()
        {
            _startTime = Time.time;
            if (SkipFrameZero)
                _startTime -= Time.deltaTime;
            _hasBeenStarted = true;
        }
        public void Stop()
        => _hasBeenStarted = false;
        public float Elapsed
        {
            get
            {
                if (_hasJustFinished)
                {
                    _hasJustFinished = false;
                    return Duration;
                }
                return Time.time - _startTime;
            }
        }
        public float Progress
        => Elapsed / Duration;
        public bool IsActive
        {
            get
            {
                if (!_hasBeenStarted)
                    return false;
                else if (Elapsed < Duration)
                    return true;
                else
                {
                    _hasBeenStarted = false;
                    _hasJustFinished = true;
                    return true;
                }
            }

        }


        // Privates
        private float _duration;
        private bool _skipFrameZero;
        private bool _hasBeenStarted;
        private bool _hasJustFinished;
        private float _startTime;
    }
}
*/