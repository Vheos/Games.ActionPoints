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

            ;// using (QAnimatorOLD.Group(this, null, Settings.FadeInDuration, StayUp))
            {
                ;// transform.GroupAnimateLocalPosition(direction * Settings.Distance);
                ;// Get<TextMeshPro>().GroupAnimateAlpha(0f, 1f);
            }
        }
        private void StayUp()
        {
            ;//var componentProperty = QAnimatorOLD.GetUID(QAnimatorOLD.ComponentProperty.TextMeshProColor);
            ;// QAnimatorOLD.Delay(this, componentProperty, Settings.StayUpDuration, FadeOut);
            StartCoroutine(Coroutines.AfterSeconds(Settings.StayUpDuration, FadeOut));
        }
        private void FadeOut()
        => QAnimator.Animate(v => Get<TextMeshPro>().alpha += v, Get<TextMeshPro>().alpha.Neg(), Settings.FadeOutDuration, new EventInfo(this.GODeactivate));

        private void Pulse()
        => transform.AnimateLocalScaleRatio(Settings.WoundPulseScale, Settings.WoundPulseDuration,
            new OptionalParameters { Curve = Settings.WoundPulseCurve, EventInfo = new EventInfo(Pulse) });
    }
}