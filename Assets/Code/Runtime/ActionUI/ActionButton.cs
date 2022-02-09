namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using TMPro;
    using Games.Core;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.General;

    [RequireComponent(typeof(Raycastable))]
    [RequireComponent(typeof(Selectable))]
    [DisallowMultipleComponent]
    public class ActionButton : AActionUIElement<ActionButtonsWheel>
    {
        // Inspector
        [SerializeField] [Range(0.1f, 1f)] protected float _Radius;

        // Publics
        public float Radius
        => _Radius;
        public Action Action
        { get; private set; }

        // Play
        public void Initialize(ActionButtonsWheel wheel, Action action)
        {
            base.Initialize(wheel);
            Action = action;
            name = $"Button";
            BindEnableDisable(wheel);
            Get<Raycastable>().BindEnableDisable(this);
            Get<Selectable>().BindEnableDisable(this);

            _originalScale = _Radius.Mul(2).ToVector3();
            Component _visualComponent = null;
            if (action.ButtonVisuals.Sprite != null)
            {
                var spriteRenderer = Add<SpriteRenderer>();
                _visualComponent = spriteRenderer;
                spriteRenderer.sprite = action.ButtonVisuals.Sprite;
                spriteRenderer.color = action.ButtonVisuals.Color;

            }
            else
            {
                var textMeshPro = Add<TextMeshPro>();
                _visualComponent = textMeshPro;
                _originalScale = _originalScale.Mul(1f, 1.5f, 1f);
                textMeshPro.rectTransform.sizeDelta = Vector2.one.Div(1f, 1.5f);
                textMeshPro.text = action.ButtonVisuals.Text.ChooseIf(t => t.IsNotNullOrEmpty(), action.name.SplitCamelCase().ToUpper());
                textMeshPro.color = action.ButtonVisuals.Color;
                textMeshPro.fontStyle = FontStyles.Bold;
                textMeshPro.enableAutoSizing = true;
                textMeshPro.fontSizeMin = 0f;
                textMeshPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
                textMeshPro.verticalAlignment = VerticalAlignmentOptions.Middle;
            }

            Get<Raycastable>().RaycastTarget = _visualComponent;
            if (TryGet(out SelectableButtonVisuals selectableButtonVisuals))
                selectableButtonVisuals.UpdateColorComponentType();
        }
    }
}