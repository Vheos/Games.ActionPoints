namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;

    [CreateAssetMenu(fileName = nameof(ToolAnimationSet), menuName = CONTEXT_MENU_PATH + nameof(ToolAnimationSet))]
    public class ToolAnimationSet : ScriptableObject
    {
        // Constants
        protected const string CONTEXT_MENU_PATH = "Action/";

        // Inspector
        [SerializeField] protected QAnimationClip[] _Idle = new QAnimationClip[1];
        [SerializeField] protected QAnimationClip[] _Sheathe = new QAnimationClip[1];

        // Public
        public IReadOnlyList<QAnimationClip> Idle
        => _Idle;
        public IReadOnlyList<QAnimationClip> Sheathe
        => _Sheathe;
    }
}