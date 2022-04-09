namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using TMPro;
    using Games.Core;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;
    using Tools.Extensions.General;

    public class Popup : ABaseComponent
    {
        // Publics
        public void Initialize(Vector3 position, string text, Color color, float scale, float pulseRate, float pulseScale,  bool isOdd)
        {
            Get<TextMeshPro>().color = color;

            transform.localScale = transform.localScale.Mul(scale);
            transform.SetPositionAndRotation(position, CameraManager.AnyNonUI.transform.rotation);
         
            if (this.Settings().PercentSignSize > 0f)
                text += $"<size={this.Settings().PercentSignSize.ToInvariant("F2")}>%</size>";
            Get<TextMeshPro>().text = text;

            float angle = isOdd.ToSign() * this.Settings().AngleRandomRange.RandomMinMax() - 90f;
            Vector3 direction = NewUtility.PointOnCircle(angle, 1f, true).Rotate(CameraManager.AnyNonUI.transform.rotation);
            if (this.Settings().AlignTextRotationToDirection)
                transform.rotation = Quaternion.LookRotation(transform.forward, direction);

            FadeIn(direction);
            if (pulseRate > 0f)
                Pulse(pulseRate, pulseScale);
        }

        // Privates
        private void FadeIn(Vector3 direction)
        => this.NewTween()
            .SetDuration(this.Settings().FadeInDuration)
            .Position(transform.position + direction * this.Settings().Distance)
            .Alpha(ColorComponent.TextMeshPro, 1f)
            .AddEventsOnFinish(StayUp);
        private void StayUp()
        => this.NewTween()
            .SetDuration(this.Settings().StayUpDuration)
            .AddEventsOnFinish(FadeOut);
        private void FadeOut()
        => this.NewTween()
            .SetDuration(this.Settings().FadeOutDuration)
            .Alpha(ColorComponent.TextMeshPro, 0f)
            .AddEventsOnFinish(this.DestroyObject);
        private void Pulse(float rate, float scale)
        => this.NewTween(ConflictResolution.Blend)
            .SetDuration(rate.Inv())
            .SetCurveShape(CurveShape.Bounce)
            .LocalScaleRatio(scale)
            .SetLoops(3);

        // Play
        protected override void PlayDestroy()
        => this.StopGameObjectTweens();
    }
}