namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using static ActionAnimation;

    [CreateAssetMenu(fileName = nameof(ToolAnimationSet), menuName = CONTEXT_MENU_PATH + nameof(ToolAnimationSet))]
    public class ToolAnimationSet : ScriptableObject
    {
        // Constants
        protected const string CONTEXT_MENU_PATH = "Action/";

        // Inspector
        [SerializeField] protected Clip[] _Idle = new Clip[1];
        [SerializeField] protected Clip[] _Sheathe = new Clip[1];

        // Public
        public IReadOnlyList<Clip> Idle
        => _Idle;
        public IReadOnlyList<Clip> Sheathe
        => _Sheathe;
    }
}