namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using Games.Core;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;
    using System.Collections.Generic;

    public class UICursor : AUICursor
    {
        // Inspector
        [Header("Visual")]
        [SerializeField] protected ImageProperties _Idle = ImageProperties.Default;
        [SerializeField] protected ImageProperties _Pressed = ImageProperties.Default;

        // Public
        public Color Color
        { get; private set; }
        public void BindToPlayer(Player player, Color color)
        {
            player.OnPlayDestroy.SubscribeAuto(this, this.DestroyObject);
            player.OnInputMoveCursor.SubscribeAuto(this, OnInputMoveCursor);
            player.OnInputPressConfirm.SubscribeAuto(this, OnInputPressConfirm);
            player.OnInputReleaseConfirm.SubscribeAuto(this, OnInputReleaseConfirm);
            name = $"{player.name}_Cursor";

            Color = color;

            Get<Image>().color = new();
            transform.localScale = new();
            SetImageProperties(_Idle);
        }

        private void SetImageProperties(ImageProperties properties)
        {
            Get<Image>().sprite = properties.Sprite;
            this.NewTween()
                .SetDuration(0.1f)
                .SetConflictResolution(ConflictResolution.Interrupt)
                .LocalScale(properties.Scale.ToVector3())
                .ImageColor(Color * properties.ColorScale);
        }
        protected override void OnInputPressConfirm()
        {
            base.OnInputPressConfirm();
            SetImageProperties(_Pressed);
        }
        protected override void OnInputReleaseConfirm()
        {
            base.OnInputReleaseConfirm();
            SetImageProperties(_Idle);
        }
    }
}