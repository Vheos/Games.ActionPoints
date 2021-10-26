namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using TMPro;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;

    public class UIDamagePopup : ABaseComponent
    {
        // CacheComponents
        protected override System.Type[] ComponentsTypesToCache => new[]
        {
            typeof(TextMeshPro),
        };

        // Publics
        public void Initialize(Vector3 position, float damage, int wounds)
        {
            bool isWound = wounds > 0;
            float lerpAlpha = isWound ? 1f : damage / 100f;
            Get<TextMeshPro>().color = UIManager.Settings._ColorCurve.Evaluate(lerpAlpha);
            transform.localScale = transform.localScale.Mul(UIManager.Settings._SizeCurve.Evaluate(lerpAlpha));
            transform.position = position;
            transform.rotation = CameraManager.FirstActive.transform.rotation;

            Get<TextMeshPro>().text = damage.RoundDown().ToString();
            if (UIManager.Settings._PercentSignSize > 0f)
                Get<TextMeshPro>().text += $"<size={UIManager.Settings._PercentSignSize.ToInvariant("F2")}>%</size>";

            FadeIn();
            if (isWound)
                Pulse();
        }

        // Privates
        private void FadeIn()
        {
            Vector2 localDirection = NewUtility.PointOnCircle(UIManager.Settings._AngleRandomRange.RandomMinMax() - 90f, 1f, true);
            Vector3 direction = localDirection.Rotate(CameraManager.FirstActive.transform.rotation);
            if (UIManager.Settings._AlignTextRotationToDirection)
                transform.localRotation = Quaternion.LookRotation(transform.forward, localDirection);

            using (QAnimator.Group(this, null, UIManager.Settings._FadeInDuration, StayUp))
            {
                transform.GroupAnimateLocalPosition(direction * UIManager.Settings._Distance);
                Get<TextMeshPro>().GroupAnimateAlpha(1f);
            }
        }
        private void StayUp()
        {
            var componentProperty = QAnimator.GetUID(QAnimator.ComponentProperty.TextMeshProAlpha);
            QAnimator.Wait(this, componentProperty, UIManager.Settings._StayUpDuration, FadeOut);
        }
        private void FadeOut()
        => Get<TextMeshPro>().AnimateAlpha(this, 0f, UIManager.Settings._FadeOutDuration, () => this.DestroyObject());
        private void Pulse()
        => transform.AnimateLocalScale(this, transform.localScale * UIManager.Settings._WoundPulseScale, UIManager.Settings._WoundPulseDuration,  Pulse, QAnimator.AnimationStyle.Boomerang);

        // Play
        public override void PlayAwake()
        {
            base.PlayAwake();
            Get<TextMeshPro>().alpha = 0f;
        }
    }
}