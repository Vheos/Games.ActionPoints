namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using System.Linq;

    [DisallowMultipleComponent]
    public class CameraManager : AComponentManager<CameraManager, Camera>
    {
        // Publics
        static public Camera AnyNonUI
        => _components.FirstOrDefault(t => !t.IsOnLayer(BuiltInLayer.UI));
    }
}