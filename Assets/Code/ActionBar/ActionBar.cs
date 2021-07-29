namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.Extensions.General;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.UtilityNS;
    public class ActionBar : AExtendedMono
    {
        private float _progress;
        private int _maxPoints;
        private int _focusPoints;

        private int ActionPoints
        => _progress.RoundDown().Clamp(0, _maxPoints);

        private bool IsExhausted
        => _progress < 0;

        private int ExhaustPoints
        => _progress.RoundUp().Clamp(-_maxPoints, 0);

        private bool IsFocusing
        => _progress > _maxPoints;

        private float FocusProgress
        => _progress.Sub(_maxPoints).Clamp(0, _maxPoints);
    }
}