namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.Utilities;
    using TMPro;

    [RequireComponent(typeof(Raycastable))]
    [RequireComponent(typeof(Selectable))]
    [DisallowMultipleComponent]
    public class ActionButton : ABaseComponent
    {
        // Inspector
        [SerializeField] [Range(0.1f, 1f)] protected float _Radius;

        // Publics
        public float Radius
        => _Radius;
        public ActionButtonsWheel Wheel
        { get; private set; }
        public Action Action
        { get; private set; }

        // Play
        public void Initialize(ActionButtonsWheel wheel, int index, Action action)
        {
            Wheel = wheel;
            Action = action;
            name = $"Button{index + 1}";
            BindEnableDisable(wheel);

            transform.localScale = _Radius.ToVector3();
            if (action.ButtonVisuals.Sprite != null)
            {
                var spriteRenderer = Add<SpriteRenderer>();
                spriteRenderer.sprite = action.ButtonVisuals.Sprite;
                spriteRenderer.color = action.ButtonVisuals.Color;
                _visualComponent = spriteRenderer;
            }
            else if (action.ButtonVisuals.Text != null)
            {
                transform.localScale = transform.localScale.Mul(1f, 1.5f, 1f);

                var textMeshPro = Add<TextMeshPro>();                
                textMeshPro.text = action.ButtonVisuals.Text;
                textMeshPro.color = action.ButtonVisuals.Color;
                textMeshPro.rectTransform.sizeDelta = Vector2.one;                
                textMeshPro.fontStyle = FontStyles.Bold;
                textMeshPro.enableAutoSizing = true;
                textMeshPro.fontSizeMin = 0f;
                textMeshPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
                textMeshPro.verticalAlignment = VerticalAlignmentOptions.Middle;
                _visualComponent = textMeshPro;
            }

            Get<Raycastable>().RaycastTarget = _visualComponent;
            if (TryGet(out SelectableButtonVisuals selectableButtonVisuals))
                selectableButtonVisuals.UpdateColorComponentType();
        }
        protected override void PlayEnable()
        {
            base.PlayEnable();
            Get<Raycastable>().Enable();
            Get<Selectable>().Enable();
        }
        protected override void PlayDisable()
        {
            base.PlayDisable();
            Get<Raycastable>().Disable();
            Get<Selectable>().Disable();
        }

        // Privates
        private Component _visualComponent;
    }
}