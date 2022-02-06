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
    using Vheos.Tools.Extensions.General;

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
        public void AnimateCreate(Vector3 targetScale)
        {
            transform.localScale = new();
            this.NewTween()
              .SetDuration(0.4f)
              .LocalScale(targetScale)
              .FinishIf(!Wheel.isActiveAndEnabled);
        }
        public void AnimateDestroy()
        {
            enabled = false;
            this.NewTween()
              .SetDuration(0.4f)
              .LocalScale(Vector3.zero)
              .OnFinish(this.DestroyObject)
              .FinishIf(!Wheel.isActiveAndEnabled);
        }
        public void AnimateMove(Vector3 targetLocalPosition)
        => this.NewTween()
            .SetDuration(0.4f)
            .LocalPosition(targetLocalPosition)
            .FinishIf(!Wheel.isActiveAndEnabled);

        // Privates
        private Component _visualComponent;

        // Play
        public void Initialize(ActionButtonsWheel wheel, Action action)
        {
            Wheel = wheel;
            Action = action;
            name = $"Button";
            BindEnableDisable(wheel);

            var targetScale = _Radius.Mul(2).ToVector3();
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
                targetScale = targetScale.Mul(1f, 1.5f, 1f);
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

            AnimateCreate(targetScale);
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
    }
}