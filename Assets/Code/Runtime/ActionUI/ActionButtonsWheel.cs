namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using Games.Core;
    using Tools.Extensions.UnityObjects;
    using Vheos.Tools.Extensions.Math;

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
            _buttons.Destroy();
            _buttons.Clear();
        }
        private void CreateButtons()
        {
            var actionsEnumerator = UI.Actionable.Actions.GetEnumerator();
            int index = 0;
            foreach (var (Position, Angle) in _positionsWheel.GetElementsPositionsAndAngles(UI.Actionable.Actions.Count, _ButtonPrefab.Radius))
                if (actionsEnumerator.MoveNext())
                {
                    var newButton = this.CreateChildComponent(_ButtonPrefab);
                    newButton.Initialize(this, index++);
                    newButton.Get<TextMeshPro>().text = actionsEnumerator.Current.Text;
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

            UI.Actionable.OnChangeActions.SubscribeAuto(this, Actionable_OnChangeActions);

            Get<Expandable>().OnStartExpanding.Subscribe(Activate);
            Get<Expandable>().OnFinishExpanding.Subscribe(Enable);
            Get<Expandable>().OnStartCollapsing.Subscribe(Disable);
            Get<Expandable>().OnFinishCollapsing.Subscribe(Deactivate);
            Get<Expandable>().ExpandTween.Set(() => this.NewTween().SetDuration(0.4f).LocalScale(Vector3.one));
            Get<Expandable>().CollapseTween.Set(() => this.NewTween().SetDuration(0.4f).LocalScale(Vector3.zero));
            Get<Expandable>().TryCollapse(true);
        }
    }
}