namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using TMPro;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;

    public class UIDamagePopup : AUpdatable
    {
        // Inspector
        public Vector2 _AngleRandomRange = new Vector2(-15, +15);
        public bool _AlignTextRotationToDirection = false;
        [Range(0f, 1f)] public float _Distance = 0.5f;
        [Range(0f, 1f)] public float _FadeInDuration = 0.5f;
        [Range(0f, 1f)] public float _StayUpDuration = 0.5f;
        [Range(0f, 1f)] public float _FadeOutDuration = 0.5f;
        public AnimationCurve _SizeCurve;
        public Gradient _ColorCurve;
        [Range(1f, 3f)] public float _WoundPulseScale = 2f;
        [Range(0f, 1f)] public float _WoundPulseDuration = 0.5f;

        // Publics
        public void Initialize(Vector3 position, float damage, int wounds)
        {
            bool isWound = wounds > 0;
            float lerpAlpha = isWound ? 1f : damage / 100f;
            _textMesh.fontSize = _SizeCurve.Evaluate(lerpAlpha);
            _textMesh.color = _ColorCurve.Evaluate(lerpAlpha);
            _textMesh.text = damage.RoundDown().ToString();

            transform.position = position;
            transform.rotation = CameraManager.FirstActive.transform.rotation;

            FadeIn();
            if (isWound)
                Pulse();
        }

        // Privates
        private TextMeshPro _textMesh;
        private void FadeIn()
        {
            Vector2 direction = NewUtility.PointOnCircle(_AngleRandomRange.RandomMinMax() - 90f, 1f, true);
            if (_AlignTextRotationToDirection)
                transform.localRotation = Quaternion.LookRotation(transform.forward, direction);
            transform.AnimateLocalPosition(this, direction * _Distance, _FadeInDuration);
            _textMesh.AnimateAlpha(this, 1f, _FadeInDuration, false, StayUp);
        }
        private void StayUp()
        => AnimationManager.Wait(this, AnimationManager.ComponentProperty.TextMeshProAlpha, _StayUpDuration, FadeOut);
        private void FadeOut()
        => _textMesh.AnimateAlpha(this, 0f, _FadeOutDuration, false, () => this.DestroyObject());
        private void Pulse()
        => transform.AnimateLocalScale(this, _WoundPulseScale.ToVector2(), _WoundPulseDuration, true, Pulse);

        // Mono
        public override void PlayAwake()
        {
            base.PlayAwake();
            _textMesh = GetComponent<TextMeshPro>();
            _textMesh.alpha = 0f;
        }
    }
}