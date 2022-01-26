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

        // Play
        /*
        protected override void PlayStart()
        {
            base.PlayStart();
            var uiCamera = InstantiateUICamera();
            var referenceResolution = _Prefab.GetComponent<CanvasScaler>().referenceResolution;
            uiCamera.transform.position = referenceResolution.Div(2f).Append(-_Prefab.planeDistance);
            uiCamera.orthographicSize = referenceResolution.y / 2f;
            foreach (var canvas in _components)
                canvas.worldCamera = uiCamera;
        }
        */
    }
}