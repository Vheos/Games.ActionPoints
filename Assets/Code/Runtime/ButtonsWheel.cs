namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;
    using System.Linq;

    // Defines
    public enum WheelPosition
    {
        Incircle,
        Excircle,
    }
    public enum ButtonPosition
    {
        TouchingPerimeterInternally,
        CenteredOnPerimeter,
        TouchingPerimeterExternally,
    }


    [DisallowMultipleComponent]
    public class ButtonsWheel : ABaseComponent
    {
        // Inspector
        [SerializeField] [Range(0f, 1f)] protected float _WheelRadiusOffset;
        [SerializeField] [Range(0.1f, 1f)] protected float _ButtonRadius;
        [SerializeField] protected WheelPosition _WheelPosition;
        [SerializeField] protected ButtonPosition _ButtonPosition;
        [SerializeField] [Range(0, 100)] protected int _MaxButtons;

        // Publics
        public float WheelRadiusOffset
        => _WheelRadiusOffset;
        public Rect ScaledRect
        => Get<Collider>().LocalBounds().Scale(transform).ToRect();
        public Vector2[] ScaledEdgePoints(params Vector2[] directions)
        {
            var r = new Vector2[directions.Length];
            Rect scaledRect = ScaledRect;
            for (int i = 0; i < directions.Length; i++)
            {
                float minY = scaledRect.size.y.Sub(scaledRect.size.x).Div(2f);
                r[i] = scaledRect.EdgePoint(directions[i]).ClampMin(float.NegativeInfinity, minY);
            }
            return r;
        }
        public (Vector2 Center, float Radius) LocalWheelCenterAndRadius
        {
            get
            {
                Vector2[] scaledEdgePoints = ScaledEdgePoints(Vector2.left, Vector2.up, Vector2.right);
                Vector2 center = NewUtility.FindCircleCenter(scaledEdgePoints);
                float radius = center.DistanceTo(_WheelPosition switch
                {
                    WheelPosition.Incircle => scaledEdgePoints.First(),
                    WheelPosition.Excircle => ScaledRect.EdgePoint(Vector2.one),
                    _ => default,
                });


                return (center, radius + _WheelRadiusOffset);
            }
        }
        public Vector2[] LocalButtonPositions
        {
            get
            {
                var (WheelCenter, WheelRadius) = LocalWheelCenterAndRadius;
                switch (_ButtonPosition)
                {
                    case ButtonPosition.TouchingPerimeterInternally: WheelRadius -= _ButtonRadius; break;
                    case ButtonPosition.TouchingPerimeterExternally: WheelRadius += _ButtonRadius; break;
                }

                float buttonAngle = _ButtonRadius.Div(WheelRadius).ArcSin().Mul(2);
                int buttonsCount = Mathf.PI.Mul(2).Div(buttonAngle).RoundDown().ClampMax(_MaxButtons);
                if (buttonsCount < 0)
                    return new Vector2[0];

                var r = new Vector2[buttonsCount];

                for (int i = 0; i < buttonsCount; i++)
                {
                    Vector2 offset = Vector2.up.Mul(WheelRadius);
                    float rotation = buttonAngle * (buttonsCount / 2 - i);
                    r[i] = WheelCenter + offset.Rotate(rotation);
                }

                return r;
            }
        }

        public float ButtonRadius
        => _ButtonRadius;
        public WheelPosition WheelPosition
        => _WheelPosition;
        public ButtonPosition ButtonPosition
        => _ButtonPosition;

    }
}

#if UNITY_EDITOR
namespace Vheos.Games.ActionPoints.Editor
{
    using System;
    using System.Linq;
    using UnityEngine;
    using UnityEditor;
    using Tools.Extensions.Math;

    public static class TransformArm_GizmoDrawer
    {
        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected | GizmoType.Pickable)]
        static void Pickable(ButtonsWheel component, GizmoType type)
        {
            if (!component.isActiveAndEnabled)
                return;

            Gizmos.color = Color.cyan;
            var (WheelCenter, WheelRadius) = component.LocalWheelCenterAndRadius;
            Gizmos.DrawWireSphere(WheelCenter.TransformNoScale(component.transform), WheelRadius);


            Gizmos.color = Color.yellow;
            foreach(var localButtonOffset in component.LocalButtonPositions)
                Gizmos.DrawWireSphere(localButtonOffset.TransformNoScale(component.transform), component.ButtonRadius);
        }
    }
}
#endif