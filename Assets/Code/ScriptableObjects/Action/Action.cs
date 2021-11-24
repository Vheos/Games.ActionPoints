namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.General;
    using Vheos.Tools.Extensions.Collections;

    [CreateAssetMenu(fileName = nameof(Action), menuName = CONTEXT_MENU_PATH + nameof(Action))]
    public class Action : ScriptableObject
    {
        // Constants
        protected const string CONTEXT_MENU_PATH = "Action/";

        // Inspector
        [Header("Visual")]
        [SerializeField] protected Sprite _Sprite = null;
        [SerializeField] protected ActionAnimation _Animation = null;
        [Header("Cost")]
        [SerializeField] [Range(0, 5)] protected int _ActionPointsCost = 0;
        [SerializeField] [Range(0, 5)] protected int _FocusPointsCost = 0;
        [Header("Use")]
        [SerializeField] protected AActionTargetTest.Data[] _TargetingANDTests = new AActionTargetTest.Data[0];
        [SerializeField] protected AActionTargetTest.Data[] _TargetingORTests = new AActionTargetTest.Data[0];
        [SerializeField] protected AActionEffect.Data[] _Effects = new AActionEffect.Data[1];

        // Publics
        public Sprite Sprite
        => _Sprite;
        public ActionAnimation Animation
        => _Animation;
        public int ActionPointsCost
        => _ActionPointsCost;
        public int FocusPointsCost
        => _FocusPointsCost;
        public bool IsTargeted
        => _TargetingANDTests.Length + _TargetingORTests.Length > 0;

        // Publics (use)
        public bool CanBeUsedBy(Actionable actionable)
        => !actionable.IsExhausted
        && actionable.ActionPointsCount + actionable.UsableMaxActionPoints >= _ActionPointsCost
        && actionable.FocusPointsCount >= _FocusPointsCost;
        public void Use(Actionable user, Targetable target)
        {
            user.ActionProgress -= _ActionPointsCost;
            user.FocusProgress -= _FocusPointsCost;
            foreach (var effectData in _Effects)
                effectData.Invoke(user, target);
        }
        public void TryPlayAnimation(ActionAnimator animator, ActionAnimation.Type type)
        {
            if (_Animation != null)
                animator.Animate(_Animation.ToClips(type));
        }
        public bool CanTarget(Targeter user, Targetable target)
        {
            foreach (var and in _TargetingANDTests)
                if (!and.Test(user, target))
                    return false;

            foreach (var or in _TargetingORTests)
                if (or.Test(user, target))
                    return true;

            if (_TargetingORTests.IsEmpty())
                return true;

            return false;
        }
    }
}