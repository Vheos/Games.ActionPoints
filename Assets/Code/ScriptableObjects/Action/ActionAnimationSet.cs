namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;

    [CreateAssetMenu(fileName = nameof(ActionAnimationSet), menuName = CONTEXT_MENU_PATH + nameof(ActionAnimationSet))]
    public class ActionAnimationSet : ScriptableObject
    {
        // Constants
        protected const string CONTEXT_MENU_PATH = "Action/";

        // Inspector
        [SerializeField] protected QAnimationClip[] _Target = new QAnimationClip[1];
        [SerializeField] protected QAnimationClip[] _Use = new QAnimationClip[1];

        // Public
        public IReadOnlyList<QAnimationClip> Target
        => _Target;
        public IReadOnlyList<QAnimationClip> Use
        => _Use;

        // Defines
        public enum Type
        {
            Target,
            Use,
            Idle,
            UseThenIdle,
        }
    }
}