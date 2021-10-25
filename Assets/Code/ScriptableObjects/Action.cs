namespace Vheos.Games.ActionPoints
{
    using UnityEngine;

    [CreateAssetMenu(fileName = nameof(Action), menuName = nameof(Action), order = 1)]
    public class Action : ScriptableObject
    {
        // Inspector
        public Sprite _Sprite = null;
        [Range(0, 5)] public int _ActionPointsCost = 1;
        [Range(0, 5)] public int _FocusPointsCost = 1;
        public bool _IsTargeted = false;
        public ActionAnimation _Animation = null;
        public AActionEffect.Data[] _EffectDataArray = new AActionEffect.Data[1];

        // Publics
        public bool CanBeUsed(Character character)
        {
            return !character.IsExhausted
                && character.ActionPointsCount + character.MaxActionPoints >= _ActionPointsCost
                && character.FocusPointsCount >= _FocusPointsCost;
        }
        public void Use(Character user, Character target)
        {
            Debug.Log($"{user.name} is using {name}" + (target != null ? $" on {target.name}" : ""));
            user.ChangeActionPoints(-_ActionPointsCost);
            user.ChangeFocusPoints(-_FocusPointsCost);
            foreach (var effectData in _EffectDataArray)
                effectData.Invoke(user, target);
        }


    }
}