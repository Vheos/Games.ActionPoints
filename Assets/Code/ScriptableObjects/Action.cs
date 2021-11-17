namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.General;
    using System.Collections.Generic;

    [CreateAssetMenu(fileName = nameof(Action), menuName = nameof(Action), order = 1)]
    public class Action : ScriptableObject
    {
        // Inspector
        [SerializeField] protected Sprite _Sprite = null;
        [SerializeField] protected ActionAnimation _Animation = null;
        [SerializeField] [Range(0, 5)] protected int _ActionPointsCost = 0;
        [SerializeField] [Range(0, 5)] protected int _FocusPointsCost = 0;
        [SerializeField] protected bool _IsTargeted = false;
        [SerializeField] protected Targetables _Targets;
        [SerializeField] protected AActionEffect.Data[] _EffectDataArray = new AActionEffect.Data[1];

        // Publics
        public Sprite Sprite
        => _Sprite;
        public int ActionPointsCost
        => _ActionPointsCost;
        public int FocusPointsCost
        => _FocusPointsCost;
        public bool IsTargeted
        => _IsTargeted;
        public ActionAnimation Animation
        => _Animation;
        public bool CanBeUsedBy(Actionable actionable)
        => !actionable.IsExhausted
        && actionable.ActionPointsCount + actionable.UsableMaxActionPoints >= _ActionPointsCost
        && actionable.FocusPointsCount >= _FocusPointsCost;
        public bool CanTarget(ActionTargetable target)
        {
            foreach (Type type in TargetableTypes)
                if (!target.Has(type))
                    return false;
            return true;
        }
        public void Use(Actionable user, ABaseComponent target)
        {
            user.ChangeActionPoints(-_ActionPointsCost);
            user.ChangeFocusPoints(-_FocusPointsCost);
            foreach (var effectData in _EffectDataArray)
                effectData.Invoke(user, target);
        }

        // Privates
        private Type[] _targetableTypes;
        private Type[] TargetableTypes
        {
            get
            {
                if (_targetableTypes == null)
                {
                    List<Type> r = new List<Type>();
                    _Targets.ForEachSetFlag(t => r.Add(GetTargetableType(t)));
                    _targetableTypes = r.ToArray();
                }
                return _targetableTypes;
            }
        }
        private Type GetTargetableType(Targetables targetable)
        => targetable switch
        {
            Targetables.Actionable => typeof(Actionable),
            Targetables.Woundable => typeof(Woundable),
            _ => null,
        };

        // Defines
        [Flags]
        protected enum Targetables
        {
            Actionable = 1 << 0,
            Woundable = 1 << 1,
        }
    }
}