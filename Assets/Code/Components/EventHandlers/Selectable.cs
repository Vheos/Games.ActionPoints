namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.General;
    using Event = Tools.UnityCore.Event;
    using static UIManager;

    [DisallowMultipleComponent]
    public class Selectable : ABaseComponent
    {
        // Events
        public Event OnGainHighlight
        { get; } = new Event();
        public Event OnLoseHighlight
        { get; } = new Event();
        public Event<ButtonFunction> OnPress
        { get; } = new Event<ButtonFunction>();
        public Event<ButtonFunction> OnHold
        { get; } = new Event<ButtonFunction>();
        public Event<ButtonFunction, bool> OnRelease
        { get; } = new Event<ButtonFunction, bool>();

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