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
        [SerializeField] [Range(0f, 2f)] protected float _TimeScale = 1f;
        [SerializeField] [Range(10, 1000)] protected int _UpdatesPerSecond = 60;
        [SerializeField] [Range(10, 100)] protected int _FixedUpdatesPerSecond = 50;
        [SerializeField] protected InputAction _Pause;

        // Private
        private void AssignInspectorValues()
        {
            Time.timeScale = _TimeScale;
            Time.fixedDeltaTime = _FixedUpdatesPerSecond.Inv();
            Application.targetFrameRate = _UpdatesPerSecond;
        }
        private void TogglePause(InputAction.CallbackContext context)
        => Time.timeScale = Time.timeScale == 0f ? _TimeScale : 0f;

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            AssignInspectorValues();            
            _Pause.Enable();
        }
        protected override void PlayEnable()
        {
            base.PlayEnable();
            _Pause.performed += TogglePause;
        }

        protected override void PlayDisable()
        {
            base.PlayDisable();
            _Pause.performed -= TogglePause;
        }
    }
}