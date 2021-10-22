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
        [Range(0f, 1f)] public float _PercentSignSize = 0.5f;

        // Publics
        public void Initialize(Vector3 position, float damage, int wounds)
        {
            bool isWound = wounds > 0;
            float lerpAlpha = isWound ? 1f : damage / 100f;
            _textMesh.color = _ColorCurve.Evaluate(lerpAlpha);
            transform.localScale = transform.localScale.Mul(_SizeCurve.Evaluate(lerpAlpha));
            transform.position = position;
            transform.rotation = CameraManager.FirstActive.transform.rotation;

            _textMesh.text = damage.RoundDown().ToString();
            if (_PercentSignSize > 0f)
                _textMesh.text += $"<size={_PercentSignSize.ToInvariant("F2")}>%</size>";
            
            FadeIn();
            if (isWound)
                Pulse();
        }

        // Privates
        private TextMeshPro _textMesh;
        private void FadeIn()
        {
            Vector2 localDirection = NewUtility.PointOnCircle(_AngleRandomRange.RandomMinMax() - 90f, 1f, true);
            Vector3 direction = localDirection.Rotate(CameraManager.FirstActive.transform.rotation);
            if (_AlignTextRotationToDirection)
                transform.localRotation = Quaternion.LookRotation(transform.forward, localDirection);
            transform.AnimateLocalPosition(this, direction * _Distance, _FadeInDuration);
            _textMesh.AnimateAlpha(this, 1f, _FadeInDuration, false, StayUp);
        }
        private void StayUp()
        => AnimationManager.Wait(this, AnimationManager.ComponentProperty.TextMeshProAlpha, _StayUpDuration, FadeOut);
        private void FadeOut()
        => _textMesh.AnimateAlpha(this, 0f, _FadeOutDuration, false, () => this.DestroyObject());
        private void Pulse()
        => transform.AnimateLocalScale(this, transform.localScale * _WoundPulseScale, _WoundPulseDuration, true, Pulse);

        // Mono
        public override void PlayAwake()
        {
            base.PlayAwake();
            _textMesh = GetComponent<TextMeshPro>();
            _textMesh.alpha = 0f;
        }
    }
}