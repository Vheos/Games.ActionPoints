namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;

    [DisallowMultipleComponent]
    sealed public class TimeManager : AManager<TimeManager>
    {
        // Inspector
        [SerializeField] [Range(0f, 2f)] private float _TimeScale = 1f;
        [SerializeField] [Range(10, 1000)] private int _UpdatesPerSecond = 60;
        [SerializeField] [Range(10, 100)] private int _FixedUpdatesPerSecond = 50;

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
        }

        // Edit
#if UNITY_EDITOR
        public override void EditInspect()
        {
            base.EditInspect();
            AssignInspectorValues();
        }
#endif
    }
}