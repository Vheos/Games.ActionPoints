namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;

    [DisallowMultipleComponent]
    public class TimeManager : AGlobalComponent<TimeManager>
    {
        // Inspector
        [SerializeField] [Range(0f, 2f)] protected float _TimeScale = 1f;
        [SerializeField] [Range(10, 1000)] protected int _UpdatesPerSecond = 60;
        [SerializeField] [Range(10, 100)] protected int _FixedUpdatesPerSecond = 50;

        // Private
        private void AssignInspectorValues()
        {
            Time.timeScale = _TimeScale;
            Time.fixedDeltaTime = _FixedUpdatesPerSecond.Inv();
            Application.targetFrameRate = _UpdatesPerSecond;
        }

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            AssignInspectorValues();
            Tween.DefaultCurve = Qurve.ValuesByProgress;
        }
    }
}