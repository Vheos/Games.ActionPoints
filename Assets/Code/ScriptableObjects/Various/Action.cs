namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.General;

    [CreateAssetMenu(fileName = nameof(Action), menuName = nameof(Action), order = 1)]
    public class Action : AInitializableSO
    {
        // Inspector
        [Header("Visual")]
        [SerializeField] protected Sprite _Sprite = null;
        [SerializeField] protected ActionAnimation _Animation = null;
        [Header("Cost")]
        [SerializeField] [Range(0, 5)] protected int _ActionPointsCost = 0;
        [SerializeField] [Range(0, 5)] protected int _FocusPointsCost = 0;
        [Header("Targeting")]
        [SerializeField] protected TargetableTypes _RequiredTypes = 0;        
        [SerializeField] protected TargetableRelations _AllowedRelations = (TargetableRelations)~0;
        [Header("Effects")]
        [SerializeField] protected AActionEffect.Data[] _EffectDataArray = new AActionEffect.Data[1];

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
        => _AllowedRelations != 0;

        // Publics (use)
        public bool CanBeUsedBy(Actionable actionable)
        => !actionable.IsExhausted
        && actionable.ActionPointsCount + actionable.UsableMaxActionPoints >= _ActionPointsCost
        && actionable.FocusPointsCount >= _FocusPointsCost;
        public void Use(Actionable user, Targetable target)
        {
            user.ActionProgress -= _ActionPointsCost;
            user.FocusProgress -= _FocusPointsCost;
            foreach (var effectData in _EffectDataArray)
                effectData.Invoke(user, target);
        }
        public void TryPlayAnimation(ActionAnimator animator, ActionAnimation.Type type)
        {
            if (_Animation != null)
                animator.Animate(_Animation.ToClips(type));
        }
        public bool CanTarget(Targeter user, Targetable target)
        {
            foreach (Type type in _requiredTypes)
                if (!target.Has(type))
                    return false;

            return _AllowedRelations.HasFlag(TargetableRelations.Self) && target.gameObject == user.gameObject
                || _AllowedRelations.HasFlag(TargetableRelations.Ally) && target.IsAllyOf(user)
                || _AllowedRelations.HasFlag(TargetableRelations.Enemy) && target.IsEnemyOf(user);
        }

        // Privates
        private List<Type> _requiredTypes;
        private List<TargetableRelations> _allowedRelations;
        private Type GetTargetableType(TargetableTypes targetableType)
        => targetableType switch
        {
            TargetableTypes.Actionable => typeof(Actionable),
            TargetableTypes.Woundable => typeof(Woundable),
            _ => null,
        };
        internal override void TryInitialize()
        {
            _requiredTypes = new List<Type>();
            _RequiredTypes.ForEachSetFlag(t => _requiredTypes.Add(GetTargetableType(t)));
            _allowedRelations = new List<TargetableRelations>();
            _AllowedRelations.ForEachSetFlag(t => _allowedRelations.Add(t));
        }

        [Flags]
        protected enum TargetableTypes
        {
            Actionable = 1 << 0,
            Woundable = 1 << 1,
        }

        [Flags]
        protected enum TargetableRelations
        {
            Self = 1 << 0,
            Ally = 1 << 1,
            Enemy = 1 << 2,
        }
    }
}