namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.General;

    [CreateAssetMenu(fileName = nameof(Action), menuName = nameof(Action), order = 1)]
    public class Action : ScriptableObject
    {
        // Inspector
        [SerializeField] protected Sprite _Sprite = null;
        [SerializeField] protected ActionAnimation _Animation = null;
        [SerializeField] [Range(0, 5)] protected int _ActionPointsCost = 0;
        [SerializeField] [Range(0, 5)] protected int _FocusPointsCost = 0;
        [SerializeField] protected bool _IsInstant = false;       
        [SerializeField] protected AActionEffect.Data[] _EffectDataArray = new AActionEffect.Data[1];

        // Publics
        public Sprite Sprite
        => _Sprite;
        public int ActionPointsCost
        => _ActionPointsCost;
        public int FocusPointsCost
        => _FocusPointsCost;
        public bool IsInstant
        => _IsInstant;
        public ActionAnimation Animation
        => _Animation;
        public bool CanBeUsed(Character character)
        {
            return !character.IsExhausted
                && character.ActionPointsCount + character.MaxActionPoints >= _ActionPointsCost
                && character.FocusPointsCount >= _FocusPointsCost;
        }
        public void Use(Character user, Character target)
        {
            user.ChangeActionPoints(-_ActionPointsCost);
            user.ChangeFocusPoints(-_FocusPointsCost);
            foreach (var effectData in _EffectDataArray)
                effectData.Invoke(user, target);
        }
    }
}