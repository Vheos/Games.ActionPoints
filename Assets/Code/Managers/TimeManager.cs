namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;

    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-1)]
    public class TimeManager : ABaseComponent
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
        public override void PlayAwake()
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