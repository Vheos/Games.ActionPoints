namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;
    using UnityEngine.InputSystem;

    [DisallowMultipleComponent]
    public class TimeManager : AStaticComponent<TimeManager>
    {
        // Inspector
        [field: SerializeField, Range(0f, 2f)] public float TimeScale { get; private set; } = 1f;
        [field: SerializeField, Range(10, 1000)] public int UpdatesPerSecond { get; private set; } = 60;
        [field: SerializeField, Range(10, 100)] public int FixedUpdatesPerSecond { get; private set; } = 50;
        [field: SerializeField] public InputAction Pause { get; private set; }

        // Private
        private void AssignInspectorValues()
        {
            Time.timeScale = TimeScale;
            Time.fixedDeltaTime = FixedUpdatesPerSecond.Inv();
            Application.targetFrameRate = UpdatesPerSecond;
        }
        private void TogglePause(InputAction.CallbackContext context)
        => Time.timeScale = Time.timeScale == 0f ? TimeScale : 0f;

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            AssignInspectorValues();
            Pause.Enable();
        }
        protected override void PlayEnable()
        {
            base.PlayEnable();
            Pause.performed += TogglePause;
        }

        protected override void PlayDisable()
        {
            base.PlayDisable();
            Pause.performed -= TogglePause;
        }
    }
}