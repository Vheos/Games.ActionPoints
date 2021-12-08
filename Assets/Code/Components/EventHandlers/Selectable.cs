namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.General;
        using static UIManager;

    [DisallowMultipleComponent]
    public class Selectable : ABaseComponent
    {
        // Events
        public AutoEvent OnGainHighlight
        { get; } = new AutoEvent();
        public AutoEvent OnLoseHighlight
        { get; } = new AutoEvent();
        public AutoEvent<ButtonFunction> OnPress
        { get; } = new AutoEvent<ButtonFunction>();
        public AutoEvent<ButtonFunction> OnHold
        { get; } = new AutoEvent<ButtonFunction>();
        public AutoEvent<ButtonFunction, bool> OnRelease
        { get; } = new AutoEvent<ButtonFunction, bool>();

        // Privates
        internal void GainHighlight()
        => OnGainHighlight?.Invoke();
        internal void LoseHighlight()
        => OnLoseHighlight?.Invoke();
        internal void Press(ButtonFunction function)
        => OnPress?.Invoke(function);
        internal void Hold(ButtonFunction function)
        => OnHold?.Invoke(function);
        internal void Release(ButtonFunction function, bool isClick)
        => OnRelease?.Invoke(function, isClick);
    }
}