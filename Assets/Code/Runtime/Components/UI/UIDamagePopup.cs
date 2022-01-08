namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using TMPro;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;
    using Tools.Extensions.General;
    using Vheos.Tools.UtilityN;

    public class UIDamagePopup : AUIComponent
    {
        // Publics
        public void Initialize(Vector3 position, float damage, bool isWound, bool isOddPopup)
        {
            float lerpAlpha = isWound ? 1f : damage / 100f;
            Get<TextMeshPro>().color = Settings.ColorCurve.Evaluate(lerpAlpha);
            transform.localScale = transform.localScale.Mul(Settings.SizeCurve.Evaluate(lerpAlpha));
            transform.position = position;
            transform.rotation = CameraManager.FirstActive.transform.rotation;

            Get<TextMeshPro>().text = damage.RoundDown().ToString();
            if (Settings.PercentSignSize > 0f)
                Get<TextMeshPro>().text += $"<size={Settings.PercentSignSize.ToInvariant("F2")}>%</size>";

            FadeIn(isOddPopup);
            if (isWound)
                Pulse();
        }

        // Privates
        private UISettings.DamagePopupSettings Settings
        => UIManager.Settings.DamagePopup;
        private void FadeIn(bool isOddPopup)
        {
            float angle = isOddPopup.ToSign() * Settings.AngleRandomRange.RandomMinMax() - 90f;
            Vector2 localDirection = NewUtility.PointOnCircle(angle, 1f, true);
            Vector3 direction = localDirection.Rotate(CameraManager.FirstActive.transform.rotation);
            if (Settings.AlignTextRotationToDirection)
                transform.localRotation = Quaternion.LookRotation(transform.forward, localDirection);

            this.NewTween()
                .SetDuration(Settings.FadeInDuration)
                .LocalPosition(direction * Settings.Distance)
                .TMPAlpha(1f)
                .AddOnFinishEvents(StayUp);

        }
        private void StayUp()
        => this.NewTween()
            .SetDuration(Settings.StayUpDuration)
            .AddOnFinishEvents(FadeOut);
        private void FadeOut()
        => this.NewTween()
            .SetDuration(Settings.FadeOutDuration)
            .AddPropertyModifier(v => Get<TextMeshPro>().alpha += v, Get<TextMeshPro>().alpha.Neg())
            .AddOnFinishEvents(this.StopTweens, this.DestroyObject);
        private void Pulse()
        => this.NewTween()
            .SetDuration(Settings.WoundPulseDuration)
            .LocalScaleRatio(Settings.WoundPulseScale)
            .SetCurveShape(CurveShape.Bounce);
    }
}