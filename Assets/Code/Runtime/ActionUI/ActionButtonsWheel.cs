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

    [RequireComponent(typeof(Expandable))]
    [DisallowMultipleComponent]
    public class ActionButtonsWheel : ABaseComponent
    {
        // Inspector
        [SerializeField] protected ActionButton _ButtonPrefab;

        // Publics
        public ActionUI UI
        { get; private set; }
        public IReadOnlyCollection<ActionButton> Buttons
        => _buttons;

        // Privates
        private readonly HashSet<ActionButton> _buttons = new();
        private RectCircle _positionsWheel;
        private void DestroyButtons()
        {
            _buttons.DestroyObject();
            _buttons.Clear();
        }
        private void CreateButtons()
        {
            var actionsEnumerator = UI.Actionable.Actions.GetEnumerator();
            int index = 0;
            foreach (var (Position, Angle) in _positionsWheel.GetElementsPositionsAndAngles(UI.Actionable.Actions.Count, _ButtonPrefab.Radius))
            {
                if (!actionsEnumerator.MoveNext())
                    break;

                if (actionsEnumerator.Current == null
                || !actionsEnumerator.Current.ButtonVisuals.HasAnyVisuals)
                    continue;

                var newButton = Instantiate(_ButtonPrefab);                
                newButton.Initialize(this, index++, actionsEnumerator.Current);
                newButton.BecomeChildOf(this);
                newButton.transform.localPosition = Position;
                _buttons.Add(newButton);
            }
        }
        private void Actionable_OnChangeActions()
        {
            DestroyButtons();
            CreateButtons();
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
            CreateButtons();

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