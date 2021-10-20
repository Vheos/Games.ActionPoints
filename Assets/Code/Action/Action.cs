namespace Vheos.Games.ActionPoints
{
    using UnityEngine;

    [CreateAssetMenu(fileName = nameof(Action), menuName = nameof(Action))]
    public class Action : ScriptableObject
    {
        // Inspector
        public Sprite _Sprite = null;
        [Range(0, 5)] public int _ActionPointsCost = 1;
        [Range(0, 5)] public int _FocusPointsCost = 1;
        public bool _IsTargeted = false;
        public AActionScript.Data[] _ScriptDataArray = new AActionScript.Data[1];

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
            foreach (var scriptValuesTuple in _ScriptDataArray)
                scriptValuesTuple.Invoke(user, target);
        }


    }
}