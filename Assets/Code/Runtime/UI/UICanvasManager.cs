namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Linq;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Collections;
    using UnityEngine.UI;
    using Tools.Extensions.Math;

    [DisallowMultipleComponent]
    public class UICanvasManager : AComponentManager<UICanvasManager, UICanvas>
    {
        // Inspector
        [SerializeField] protected Camera _UICameraPrefab;

        // Publics
        static public Camera InstantiateUICamera()
        => CameraManager.InstantiateComponent(_instance._UICameraPrefab);
    }
}