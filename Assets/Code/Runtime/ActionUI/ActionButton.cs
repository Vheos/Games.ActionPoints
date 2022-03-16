namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using TMPro;
    using Games.Core;
    using Tools.Extensions.Math;
    using Tools.Extensions.General;
    using Vheos.Tools.Extensions.Collections;

    [RequireComponent(typeof(Raycastable))]
    [RequireComponent(typeof(Selectable))]
    [RequireComponent(typeof(CommonSelectable))]
    [RequireComponent(typeof(Updatable))]
    [DisallowMultipleComponent]
    public class ActionButton : AActionUIElement<ActionButtonsWheel>
    {
        // Publics
        public Action Action
        { get; private set; }

        // Privates
        private bool _isUsable;
        private ColorComponent _colorComponentType;
        private ActionTargeter ActionTargeter
        => _group.UI.Actionable.Get<ActionTargeter>();
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
        private void Selectable_OnGainSelection(Selecter selecter, bool isFirst)
        {
            if (!_isUsable || !isFirst)
                return;

            ActionTargeter.TryStartHighlightingValidTargets(Action);
        }
        private void Selectable_OnLoseSelection(Selecter selecter, bool isLast)
        {
            if (!_isUsable || !isLast)
                return;

            ActionTargeter.TryStopHighlightingValidTargets();
        }
        private void Selectable_OnRelease(Selecter selecter, bool isFullClick)
        {
            if (!_isUsable || !isFullClick)
                return;

            _group.UI.Actionable.Get<ActionTargeter>().UseAction(Action, transform);
        }
        private void PlayerOwnable_OnChangePlayer(Player from, Player to)
        {
            switch (_colorComponentType)
            {
                case ColorComponent.SpriteRenderer:
                    Get<SpriteRenderer>().color = to.Color;
                    break;
                case ColorComponent.TextMeshPro:
                    Get<TextMeshPro>().color = to.Color;
                    break;
            }
            AnimateUsability(_isUsable, true);
        }
        private void UpdateUsability(bool instantly = false)
        {
            bool previousIsUsable = _isUsable;
            _isUsable = _group.UI.Actionable.CanAfford(Action)
                     && TargetableManager.GetValidTargets(_group.UI.Actionable, Action).IsNotEmpty();
            if (_isUsable == previousIsUsable)
                return;

            AnimateUsability(_isUsable, instantly);
            Get<Selectable>().ClearSelectionAndHolder();
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
            name = action.name;
            BindEnableDisable(wheel);
            Get<Raycastable>().BindEnableDisable(this);
            Get<Selectable>().BindEnableDisable(this);

            _originalScale = this.Settings().Radius.Mul(2).ToVector3();
            if (action.ButtonVisuals.Sprite != null)
            {
                Get<Raycastable>().RaycastTarget = CreateSpriteRenderer();
                _colorComponentType = ColorComponent.SpriteRenderer;
            }
            else
            {
                Get<Raycastable>().RaycastTarget = CreateTextMeshPro();
                _colorComponentType = ColorComponent.TextMeshPro;
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

            Get<Selectable>().OnGainSelection.SubEnableDisable(this, Selectable_OnGainSelection);
            Get<Selectable>().OnLoseSelection.SubEnableDisable(this, Selectable_OnLoseSelection);
            Get<Selectable>().OnRelease.SubEnableDisable(this, Selectable_OnRelease);

            Get<Selectable>().AddSelectionTest(selecter => _group.UI.Actionable.HasPlayerOwner(selecter.Get<Player>()));
            if (_group.UI.Actionable.TryGet(out PlayerOwnable playerOwnable))
                playerOwnable.OnChangeOwner.SubDestroy(this, PlayerOwnable_OnChangePlayer);
        }
    }
}

/*
// transparent when cursor is far away
private void Updatable_OnUpdate()
{
    Get<Updatable>().OnUpdate.SubEnableDisable(this, Updatable_OnUpdate);

    if (!PlayerManager.TryGetAnyActive(out var player))
        return;

    Vector2 buttonScreenPosition = UICanvasManager.Any.ScreenPosition(this);
    Vector2 cursorScreenPosition = UICanvasManager.Any.ScreenPosition(player.Cursor);
    float alpha = buttonScreenPosition.DistanceTo(cursorScreenPosition).MapClamped(50, 400, 1f, 0.2f);
    Debug.Log($"{name} - {   alpha:P}");

    switch (_colorComponentType)
    {
        case ColorComponentType.SpriteRenderer:
            Get<SpriteRenderer>().color = Get<SpriteRenderer>().color.NewA(alpha);
            break;
        case ColorComponentType.TextMeshPro:
            Get<TextMeshPro>().alpha = alpha;
            break;
    }
}
 */