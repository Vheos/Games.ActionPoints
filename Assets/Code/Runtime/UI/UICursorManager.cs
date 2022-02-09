namespace Vheos.Games.ActionPoints
{
    using System;
    using Games.Core;
    using UnityEngine;

    [DisallowMultipleComponent]
    public class UICursorManager : AStaticManager<UICursorManager, UICursor>
    {
        // Inspector
        [field: SerializeField] public bool DisableNativeCursor { get; private set; }

        protected override void PlayAwake()
        {
            base.PlayAwake();
            if (DisableNativeCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}