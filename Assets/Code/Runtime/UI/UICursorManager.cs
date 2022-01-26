namespace Vheos.Games.ActionPoints
{
    using System;
    using Games.Core;
    using UnityEngine;

    [DisallowMultipleComponent]
    public class UICursorManager : AComponentManager<UICursorManager, UICursor>
    {
        // Inspector
        [SerializeField] protected bool _DisableNativeCursor;

        protected override void PlayAwake()
        {
            base.PlayAwake();
            if (_DisableNativeCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}