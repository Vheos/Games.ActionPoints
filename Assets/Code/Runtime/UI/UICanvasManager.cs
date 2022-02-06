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
    public class UICanvasManager : AStaticManager<UICanvasManager, UICanvas>
    {
        // Inspector
        [SerializeField] protected CCamera _UICameraPrefab;

        // Publics
        static public CCamera InstantiateUICamera()
        => CameraManager.InstantiateComponent(_instance._UICameraPrefab);
    }
}