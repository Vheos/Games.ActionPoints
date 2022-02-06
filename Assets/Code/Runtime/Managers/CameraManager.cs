namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using System.Linq;

    [DisallowMultipleComponent]
    public class CameraManager : AStaticManager<CameraManager, CCamera>
    {
        // Publics
        static public CCamera AnyNonUI
        => _components.FirstOrDefault(t => !t.IsOnLayer(BuiltInLayer.UI));
    }
}