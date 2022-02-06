namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using Games.Core;
    using Tools.Extensions.UnityObjects;
    using Vheos.Tools.Extensions.Math;
    using System.Linq;
    using Vheos.Tools.Extensions.General;

    [RequireComponent(typeof(Expandable))]
    [DisallowMultipleComponent]
    public class ActionButtonsWheel : ABaseComponent
    {
        // Publics
        public ActionUI UI
        { get; private set; }
        public IReadOnlyCollection<ActionButton> Buttons
        => _buttons;

        // Privates
        private readonly HashSet<ActionButton> _buttons = new();
        private RectCircle _positionsWheel;
        private bool TryGetButtonForAction(Action action, out ActionButton button)
        => _buttons.FirstOrDefault(t => t.Action == action).TryNonNull(out button);
        private void DestroyButtons(IEnumerable<Action> removedActions)
        {
            foreach (var action in removedActions)
                if (TryGetButtonForAction(action, out var button))
                {
                    _buttons.Remove(button);
                    button.AnimateDestroy();
                }
        }
        private void CreateButtons(IEnumerable<Action> addedActions)
        {
            foreach (var action in addedActions)
            {
                var newButton = Instantiate(Settings.Prefabs.ActionButton);
                _buttons.Add(newButton);
                newButton.Initialize(this, action);
                newButton.BecomeChildOf(this);               
            }
        }
        private IEnumerable<ActionButton> GetSortedButtons(IEnumerable<ActionButton> buttons)
        {
            var r = buttons.ToArray();
            r.Shuffle();
            return r;
        }
        private void UpdateButtonPositions()
        {
            var buttonTransforms = _positionsWheel.GetElementsPositionsAndAngles(UI.Actionable.Actions.Count, Settings.Prefabs.ActionButton.Radius).GetEnumerator();
            foreach (var button in GetSortedButtons(_buttons))
                if (buttonTransforms.MoveNext())
                {
                    button.AnimateMove(buttonTransforms.Current.Position);
                }

        }
        private void Actionable_OnChangeActions(IEnumerable<Action> removedActions, IEnumerable<Action> addedActions)
        {
            foreach (var action in removedActions)
                Debug.Log($"-- {action.name}");
            foreach (var action in addedActions)
                Debug.Log($"++ {action.name}");
            Debug.Log($"SettingsTest: {Settings.Visual.ActionButton.TestFloat}");
            Debug.Log($"");


            DestroyButtons(removedActions);
            CreateButtons(addedActions);
            UpdateButtonPositions();
        }

        // Play
        public void Initialize(ActionUI ui)
        {
            UI = ui;
            name = $"ButtonsWheel";
            BindEnableDisable(ui);

            _positionsWheel = new()
            {
                Rect = UI.Rect,
                EncapsulateClosestCorner = true,
                Angle = -90f,
            };

            UI.Actionable.OnChangeActions.SubDestroy(this, Actionable_OnChangeActions);

            Get<Expandable>().OnStartExpanding.SubDestroy(this, Activate);
            Get<Expandable>().OnFinishExpanding.SubDestroy(this, Enable);
            Get<Expandable>().OnStartCollapsing.SubDestroy(this, Disable);
            Get<Expandable>().OnFinishCollapsing.SubDestroy(this, Deactivate);
            Get<Expandable>().ExpandTween.Set(() => this.NewTween().SetDuration(0.4f).LocalScale(Vector3.one));
            Get<Expandable>().CollapseTween.Set(() => this.NewTween().SetDuration(0.4f).LocalScale(Vector3.zero));
            Get<Expandable>().TryCollapse(true);
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