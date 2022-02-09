namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    [CreateAssetMenu(fileName = nameof(Action), menuName = CONTEXT_MENU_PATH + nameof(Action))]
    public class Action : ScriptableObject
    {
        // Constants
        protected const string CONTEXT_MENU_PATH = "";

        // Inspector
        [field: SerializeField] public ActionButtonVisuals ButtonVisuals { get; private set; }
        [field: SerializeField, Range(0, 5)] public int ActionPointsCost { get; private set; }
        [field: SerializeField, Range(0, 5)] public int FocusPointsCost { get; private set; }
        [field: SerializeField] public ActionEffectData[] Effects { get; private set; }

        // Publics (use)
        public void Use(Actionable user, Targetable target)
        {
            user.ActionProgress -= ActionPointsCost;
            user.FocusProgress -= FocusPointsCost;
            foreach (var effectData in Effects)
                effectData.Invoke(user, target);
        }
    }
}