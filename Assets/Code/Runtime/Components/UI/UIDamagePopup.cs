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

            QAnimator.Animate(Settings.FadeInDuration)
                .LocalPosition(transform, direction * Settings.Distance)
                .Alpha(Get<TextMeshPro>(), 1f)
                .Events(StayUp);

        }
        private void StayUp()
        => QAnimator.Animate(Settings.StayUpDuration)
            .Events(FadeOut);
        private void FadeOut()
        => this.Animate(Settings.FadeOutDuration)
            .Custom(v => Get<TextMeshPro>().alpha += v, Get<TextMeshPro>().alpha.Neg())
            .Events(this.StopAnimations, this.DestroyObject);
        private void Pulse()
        => this.Animate(Settings.WoundPulseDuration)
            .LocalScaleRatio(transform, Settings.WoundPulseScale)
            .Set(CurveFuncType.Bounce);
    }
}