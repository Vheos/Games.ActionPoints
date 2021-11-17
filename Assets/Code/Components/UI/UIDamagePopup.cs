namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using TMPro;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;
    using Tools.Extensions.General;

    public class UIDamagePopup : AUIComponent
    {
        // Publics
        public void Initialize(Vector3 position, float damage, bool isWound)
        {
            float lerpAlpha = isWound ? 1f : damage / 100f;
            Get<TextMeshPro>().color = Settings.ColorCurve.Evaluate(lerpAlpha);
            transform.localScale = transform.localScale.Mul(Settings.SizeCurve.Evaluate(lerpAlpha));
            transform.position = position;
            transform.rotation = CameraManager.FirstActive.transform.rotation;

            Get<TextMeshPro>().text = damage.RoundDown().ToString();
            if (Settings.PercentSignSize > 0f)
                Get<TextMeshPro>().text += $"<size={Settings.PercentSignSize.ToInvariant("F2")}>%</size>";

            FadeIn();
            if (isWound)
                Pulse();
        }

        // Privates
        private UISettings.DamagePopupSettings Settings
        => UIManager.Settings.DamagePopup;
        private void FadeIn()
        {
            Vector2 localDirection = NewUtility.PointOnCircle(Settings.AngleRandomRange.RandomMinMax() - 90f, 1f, true);
            Vector3 direction = localDirection.Rotate(CameraManager.FirstActive.transform.rotation);
            if (Settings.AlignTextRotationToDirection)
                transform.localRotation = Quaternion.LookRotation(transform.forward, localDirection);

            using (QAnimator.Group(this, null, Settings.FadeInDuration, StayUp))
            {
                transform.GroupAnimateLocalPosition(direction * Settings.Distance);
                Get<TextMeshPro>().GroupAnimateAlpha(0f, 1f);
            }
        }
        private void StayUp()
        {
            var componentProperty = QAnimator.GetUID(QAnimator.ComponentProperty.TextMeshProColor);
            QAnimator.Delay(this, componentProperty, Settings.StayUpDuration, FadeOut);
        }
        private void FadeOut()
        => Get<TextMeshPro>().AnimateAlpha(this, 0f, Settings.FadeOutDuration, () => this.DestroyObject());
        private void Pulse()
        => transform.AnimateLocalScale(this, transform.localScale * Settings.WoundPulseScale, Settings.WoundPulseDuration, Pulse, QAnimator.Curve.Boomerang);
    }
}