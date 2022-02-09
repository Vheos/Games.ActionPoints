namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Linq;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;

    [DisallowMultipleComponent]
    public class RaycastableManager : AStaticManager<RaycastableManager, Raycastable>
    {
        static public T FindClosest<T>(UICanvas uiCanvas, Vector2 canvasPosition) where T : Component
        {
            T closestComponent = null;
            float minDistance = float.PositiveInfinity;
            bool raycastOnlyUI = false;
            foreach (var raycastable in ActiveComponents.Where(t => t.Has<T>()))
            {
                bool isUI = raycastable.IsOnLayer(BuiltInLayer.UI);
                if (raycastOnlyUI && !isUI)
                    continue;

                CCamera camera = isUI ? uiCanvas.CanvasCamera : uiCanvas.WorldCamera;
                if (raycastable.Collider.Raycast(camera.Unity.ScreenPointToRay(uiCanvas.CanvasToScreenPosition(canvasPosition)), out var hitInfo, float.PositiveInfinity)
                && raycastable.PerformRaycastTests(hitInfo.point))
                {
                    float distance = hitInfo.point.DistanceTo(camera);
                    if (distance < minDistance)
                    {
                        closestComponent = raycastable.Get<T>();
                        minDistance = distance;
                        if (isUI)
                            raycastOnlyUI = true;
                    }
                }
            }
            return closestComponent;
        }
        static public T FindClosest<T>(UICanvas uiCanvas, GameObject pointer) where T : Component
        => FindClosest<T>(uiCanvas, uiCanvas.CanvasPosition(pointer));
        static public T FindClosest<T>(UICanvas uiCanvas, Component pointer) where T : Component
        => FindClosest<T>(uiCanvas, uiCanvas.CanvasPosition(pointer));
    }
}