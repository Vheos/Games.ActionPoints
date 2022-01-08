namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;

    [RequireComponent(typeof(SpriteRenderer))]
    public class UIWound : AUIComponent
    {
        // Publics
        public void Initialize()
        {
            Get<SpriteRenderer>().color = UIManager.Settings.ActionPoint.ExhaustColor;
            Hide(true);
        }
        public void Show(int index)
        {
            this.GOActivate();
            float randFrom = index.IsEven() ? Settings.WoundAngleRandomRange.AvgComp() : Settings.WoundAngleRandomRange.x;
            float randTo = index.IsEven() ? Settings.WoundAngleRandomRange.y : Settings.WoundAngleRandomRange.AvgComp();
            transform.localRotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(randFrom, randTo).Neg());
            transform.localPosition = Vector2.up.Mul(Settings.FadeDistance).Rotate(transform.localRotation);

            this.NewTween()
                .SetDuration(Settings.WoundAnimDuration)
                .SetConflictResolution(ConflictResolution.Interrupt)
                .LocalPosition(Vector3.zero)
                .SpriteAlpha(1f);
        }
        public void Hide(bool isInstant = false)
        {
            Vector2 fadeOutPosition = Vector2.right.Mul(Settings.FadeDistance).Rotate(transform.localRotation);
            this.NewTween()
                .SetDuration(isInstant ? 0f : Settings.WoundAnimDuration)
                .SetConflictResolution(ConflictResolution.Interrupt)
                .LocalPosition(fadeOutPosition)
                .SpriteAlpha(0f)
                .AddOnFinishEvents(this.GODeactivate);
        }

        // Privates
        private UISettings.WoundSettings Settings
        => UIManager.Settings.Wound;
    }
}