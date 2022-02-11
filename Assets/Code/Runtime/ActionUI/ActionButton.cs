namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using TMPro;
    using Games.Core;
    using Tools.Extensions.Math;
    using Tools.Extensions.General;

    [RequireComponent(typeof(Raycastable))]
    [RequireComponent(typeof(Selectable))]
    [RequireComponent(typeof(CommonSelectable))]
    [DisallowMultipleComponent]
    public class ActionButton : AActionUIElement<ActionButtonsWheel>
    {
        // Publics
        public Action Action
        { get; private set; }

        // Privates
        private bool _isUsable;
        private ColorComponentType _colorComponentType;
        private SpriteRenderer CreateSpriteRenderer()
        {
            var spriteRenderer = Add<SpriteRenderer>();
            spriteRenderer.sprite = Action.ButtonVisuals.Sprite;
            spriteRenderer.color = Action.ButtonVisuals.Color;
            return spriteRenderer;
        }
        private TextMeshPro CreateTextMeshPro()
        {
            var textMeshPro = Add<TextMeshPro>();
            textMeshPro.rectTransform.sizeDelta = Vector2.one.Div(1f, 1.5f);
            textMeshPro.text = Action.ButtonVisuals.Text.ChooseIf(t => t.IsNotNullOrEmpty(), Action.name.SplitCamelCase().ToUpper());
            textMeshPro.color = Action.ButtonVisuals.Color;
            textMeshPro.fontStyle = FontStyles.Bold;
            textMeshPro.enableAutoSizing = true;
            textMeshPro.fontSizeMin = 0f;
            textMeshPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
            textMeshPro.verticalAlignment = VerticalAlignmentOptions.Middle;
            return textMeshPro;
        }
        private void Selectable_OnPress(Selecter selecter)
        {
            if (!_isUsable)
                return;

            _group.UI.Actionable.Get<ActionTargeter>().ShowTargetingLine(selecter, Action, transform);
        }
        private void Selectable_OnRelease(Selecter selecter, bool isFullClick)
        {
            if (!_isUsable)
                return;

            _group.UI.Actionable.Get<ActionTargeter>().HideTargetingLine(selecter);
        }
        private void UpdateUsability(bool instantly = false)
        {
            bool previousIsUsable = _isUsable;
            _isUsable = _group.UI.Actionable.CanAfford(Action);
            if (_isUsable == previousIsUsable)
                return;

            AnimateUsability(_isUsable, instantly);
            Get<CommonSelectable>().IsEnabled = _isUsable;
        }
        private void AnimateUsability(bool state, bool instantly = false)
        {
            float targetColorScale = this.Settings().UnusableColorScale;
            if (state)
                targetColorScale.SetInv();

            this.NewTween()
              .SetDuration(this.Settings().ChangeUsabilityDuration)
              .ColorRatio(_colorComponentType, targetColorScale)
              .FinishIf(instantly);
        }

        // Play
        public void Initialize(ActionButtonsWheel wheel, Action action)
        {
            base.Initialize(wheel);
            Action = action;
            name = $"Button";
            BindEnableDisable(wheel);
            Get<Raycastable>().BindEnableDisable(this);
            Get<Selectable>().BindEnableDisable(this);

            _originalScale = this.Settings().Radius.Mul(2).ToVector3();
            if (action.ButtonVisuals.Sprite != null)
            {
                Get<Raycastable>().RaycastTarget = CreateSpriteRenderer();
                _colorComponentType = ColorComponentType.SpriteRenderer;
            }
            else
            {
                Get<Raycastable>().RaycastTarget = CreateTextMeshPro();
                _colorComponentType = ColorComponentType.TextMeshPro;
                _originalScale = _originalScale.Mul(1f, 1.5f, 1f);
            }

            Get<CommonSelectable>().HighlightScale.Set(() => this.Settings().HighlightScale);
            Get<CommonSelectable>().PressScale.Set(() => this.Settings().PressScale);
            Get<CommonSelectable>().UpdateColorComponentType();

            OnPlayEnable.SubDestroy(this, () => UpdateUsability());
            _group.UI.Actionable.OnChangeActionPoints.SubEnableDisable(this, (from, to) => UpdateUsability());
            _group.UI.Actionable.OnChangeFocusPoints.SubEnableDisable(this, (from, to) => UpdateUsability());
            _group.UI.Actionable.OnChangeExhausted.SubEnableDisable(this, isExhausted => UpdateUsability());
            _isUsable = true;
            UpdateUsability(true);

            Get<Selectable>().OnPress.SubEnableDisable(this, Selectable_OnPress);
            Get<Selectable>().OnRelease.SubEnableDisable(this, Selectable_OnRelease);
        }
    }
}