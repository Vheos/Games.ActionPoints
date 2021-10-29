﻿namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.Math;

    public class LightFlicker : ABaseComponent
    {
        // Inspector
        [SerializeField] protected Vector2 _IntervalRandRange = new Vector2(0.1f, 0.4f);
        [SerializeField] [Range(0f, 1f)] protected float _MinMultiplier = 0.9f;
        [SerializeField] [Range(1f, 2f)] protected float _MaxMultiplier = 1.1f;
        [SerializeField] [Range(0f, 1f)] protected float _FadeHalfTime = 0.5f;

        // CacheComponents
        protected override System.Type[] ComponentsTypesToCache => new[]
        {
            typeof(Light),
        };

        // Privates
        private float _originalIntensity;
        private float _targetIntensity;
        private float _nextFlickerTime;
        private float CurrentIntensity
        {
            get => Get<Light>().intensity;
            set => Get<Light>().intensity = value;
        }

        // Play
        public override void PlayAwake()
        {
            base.PlayAwake();
            _originalIntensity = CurrentIntensity;
        }
        protected override void SubscribeToPlayEvents()
        {
            base.SubscribeToPlayEvents();
            Updatable.OnPlayUpdate += () =>
            {
                float lerpAlpha = NewUtility.LerpHalfTimeToAlpha(_FadeHalfTime);
                _targetIntensity.SetLerp(_originalIntensity * _MinMultiplier, lerpAlpha);
                if (Time.time >= _nextFlickerTime)
                {
                    _targetIntensity = Random.Range(CurrentIntensity, _originalIntensity * _MaxMultiplier);
                    _nextFlickerTime = Time.time + _IntervalRandRange.RandomMinMax();
                }
                CurrentIntensity = CurrentIntensity.Lerp(_targetIntensity, lerpAlpha);
            };
        }
    }
}