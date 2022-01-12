namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;

    [CreateAssetMenu(fileName = nameof(Action), menuName = CONTEXT_MENU_PATH + nameof(Action))]
    public class Action : ScriptableObject
    {
        // Constants
        protected const string CONTEXT_MENU_PATH = "";

        // Inspector
        [Header("Visual")]
        [SerializeField] protected Sprite _Sprite = null;
        [Header("Cost")]
        [SerializeField] [Range(0, 5)] protected int _ActionPointsCost = 0;
        [SerializeField] [Range(0, 5)] protected int _FocusPointsCost = 0;
        [Header("Use")]
        [SerializeField] protected ActionEffectData[] _Effects = new ActionEffectData[1];

        // Publics
        public Sprite Sprite
        => _Sprite;
        public int ActionPointsCost
        => _ActionPointsCost;
        public int FocusPointsCost
        => _FocusPointsCost;

        // Publics (use)
        public bool CanBeUsedBy(Actionable actionable)
        => !actionable.IsExhausted
        && actionable.UsableActionPoints >= _ActionPointsCost
        && actionable.FocusPoints >= _FocusPointsCost;
        public void Use(Actionable user, Targetable target)
        {
            user.ActionProgress -= _ActionPointsCost;
            user.FocusProgress -= _FocusPointsCost;
            foreach (var effectData in _Effects)
                effectData.Invoke(user, target);
        }
    }
}