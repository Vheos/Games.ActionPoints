namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.UnityObjects;
    using System.Linq;

    [DisallowMultipleComponent]
    public class RaycastableManager : AComponentManager<RaycastableManager, Raycastable>
    {
        static public T FindClosest<T>(UICanvas uiCanvas, Vector2 canvasPosition) where T : Component
        {
            T closestComponent = null;
            float minDistance = float.PositiveInfinity;
            bool raycastOnlyUI = false;
            foreach (var cursorable in ActiveComponents.Where(t => t.Has<T>()))
            {
                bool isUI = cursorable.IsOnLayer(BuiltInLayer.UI);
                if (raycastOnlyUI && !isUI)
                    continue;

                Camera camera = isUI ? uiCanvas.CanvasCamera : uiCanvas.WorldCamera;
                float distance = cursorable.DistanceTo(camera);
                if (distance < minDistance
                && cursorable.Trigger.Raycast(camera.ScreenPointToRay(uiCanvas.CanvasToScreenPosition(canvasPosition)), out var hitInfo, float.PositiveInfinity)
                && cursorable.PerformRaycastTests(hitInfo.point))
                {
                    closestComponent = cursorable.Get<T>();
                    minDistance = distance;
                    if (isUI)
                        raycastOnlyUI = true;
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