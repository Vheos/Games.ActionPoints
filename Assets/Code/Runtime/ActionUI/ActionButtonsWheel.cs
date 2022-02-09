namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.General;

    [RequireComponent(typeof(Expandable))]
    [DisallowMultipleComponent]
    public class ActionButtonsWheel : AActionUIElementsGroup<ActionButton>
    {
        // Privates
        private RectCircle _positionsWheel;
        private bool TryGetButtonForAction(Action action, out ActionButton button)
        => _elements.FirstOrDefault(t => t.Action == action).TryNonNull(out button);
        private IEnumerable<ActionButton> GetSortedButtons(IEnumerable<ActionButton> buttons)
        {
            var r = buttons.ToArray();
            r.Shuffle();
            return r;
        }

        // Common
        private void UpdateButtonsCount(IEnumerable<Action> removedActions, IEnumerable<Action> addedActions)
        {
            DestroyButtons(removedActions);
            CreateButtons(addedActions);
            UpdateButtonPositions();
            _newElements.Clear();
        }
        private void DestroyButtons(IEnumerable<Action> removedActions)
        {
            foreach (var action in removedActions)
                if (TryGetButtonForAction(action, out var button))
                {
                    button.AnimateDestroy(!isActiveAndEnabled);
                    _elements.Remove(button);
                }
        }
        private void CreateButtons(IEnumerable<Action> addedActions)
        {
            foreach (var action in addedActions)
            {
                var newButton = Instantiate(SettingsManager.Prefabs.ActionButton);
                newButton.BecomeChildOf(this);
                newButton.Initialize(this, action);
                newButton.AnimateCreate(!isActiveAndEnabled);
                _newElements.Add(newButton);
            }
            _elements.AddRange(_newElements);
        }
        private void UpdateButtonPositions()
        {
            var buttonTransforms = _positionsWheel.GetElementsPositionsAndAngles(UI.Actionable.Actions.Count, this.Settings().Radius).GetEnumerator();
            foreach (var button in GetSortedButtons(_elements))
                if (buttonTransforms.MoveNext())
                {
                    bool isNew = _newElements.Contains(button);
                    button.AnimateMove(buttonTransforms.Current.Position, !isActiveAndEnabled || isNew);
                }
        }

        // Play
        override public void Initialize(ActionUI ui)
        {
            base.Initialize(ui);

            _positionsWheel = new()
            {
                Rect = UI.Rect,
                EncapsulateClosestCorner = true,
                Angle = -90f,
            };

            UI.Actionable.OnChangeActions.SubDestroy(this, UpdateButtonsCount);

            Get<Expandable>().OnFinishExpanding.SubDestroy(this, Enable);
            Get<Expandable>().OnStartCollapsing.SubDestroy(this, Disable);
        }
    }
}

/*
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
        static void Pickable(ActionButtonsWheel component, GizmoType type)
        {
            if (!component.isActiveAndEnabled)
                return;
            Gizmos.color = Color.magenta;
            /*
            foreach (var scaledEdgePoint in component._positionsWheel.EdgePoints)
                Gizmos.DrawWireSphere(scaledEdgePoint.TransformNoScale(component.transform), 0.05f);
            Gizmos.color = Color.cyan;
            var (WheelCenter, WheelRadius) = component._positionsWheel.Circle;
            Gizmos.DrawWireSphere(WheelCenter.TransformNoScale(component.transform), WheelRadius);
            
            Gizmos.color = Color.yellow;
            foreach (var (Position, Angle) in component._positionsWheel.GetElementsPositionsAndAngles(component.UI.Actionable.Actions.Count, 1/3f))
                Gizmos.DrawWireSphere(Position.TransformNoScale(component.transform), 1 / 3f);
        }
    }
}
#endif
            */