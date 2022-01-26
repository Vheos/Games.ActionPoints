namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Collections;
    using System.Linq;
    using Vheos.Tools.Extensions.General;

    [DisallowMultipleComponent]
    public class CameraManager : AComponentManager<CameraManager, Camera>
    {
        // Inspector
        [SerializeField] protected Camera _UICameraPrefab;

        // Publics
        static public Camera AnyNonUI
        => _components.FirstOrDefault(t => !t.IsOnLayer(BuiltInLayer.UI));
    }
}