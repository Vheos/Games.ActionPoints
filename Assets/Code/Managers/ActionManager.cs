namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.General;

    sealed public class ActionManager : AManager<ActionManager>
    {
        // Inspector
        [SerializeField] [Range(0f, 2f)] private float _GlobalSpeedScale = 1f;
        [SerializeField] private KeyCode _CombatPauseButton = KeyCode.Space;

        // Publics
        static public float GlobalSpeedScale
        => _instance._GlobalSpeedScale;
        static public bool IsCombatPaused
        {
            get => _isCombatPaused;
            set
            {
                _isCombatPaused = value;
                _instance._GlobalSpeedScale = 1 - _isCombatPaused.To01();
            }
        }
        static public void ToggleCombatPause()
        => IsCombatPaused = !IsCombatPaused;

        // Private
        static private bool _isCombatPaused;
        static private void OnUpdate()
        {
            if (_instance._CombatPauseButton.Pressed())
                ToggleCombatPause();
        }

        // Play
        protected override void DefineAutoSubscriptions()
        {
            base.DefineAutoSubscriptions();
            SubscribeTo(Get<Updatable>().OnUpdated, OnUpdate);
        }
        protected override void PlayAwake()
        {
            base.PlayAwake();
            _isCombatPaused = false;
        }
    }
}