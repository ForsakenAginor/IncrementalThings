using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Source.Scripts.DI.Services.Game.FSE
{
    public class ClosingEyesEffectController
    {
        private const string RadiusParameter = "_Radius";
        private const string SoftnessParameter = "_Smoothness";
        private const float MinRadius = 0f;
        private const float MinSmoothness = 0f;
        private const float MaxRadius = 1f;
        private const float MaxSmoothness = 0.75f;
        private const float TweenRadiusDuration = 2f;
        private const float TweenSmoothnessDuration = 1f;

        private readonly ScriptableRendererFeature _feature;
        private readonly Material _material;

        private Sequence _sequence;

        public ClosingEyesEffectController(ScriptableRendererFeature feature, Material material)
        {
            _feature = feature != null ? feature : throw new System.ArgumentNullException(nameof(feature));
            _material = material != null ? material : throw new System.ArgumentNullException(nameof(material));
            _feature.SetActive(false);
        }

        public void Enable()
        {
            _material.SetFloat(RadiusParameter, MinRadius);
            _material.SetFloat(SoftnessParameter, MinSmoothness);
            _feature.SetActive(true);
        }

        public void Disable()
        {
            _material.SetFloat(RadiusParameter, MinRadius);
            _material.SetFloat(SoftnessParameter, MinSmoothness);
            _feature.SetActive(false);
        }

        public void CloseEyes()
        {
            if(_feature.isActive == false)
                throw new System.InvalidOperationException("Cannot close eyes when the feature is not active.");
        
            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            // Изменяем радиус от минимального до максимального значения
            _sequence.Join(_material.DOFloat(MaxRadius, RadiusParameter, TweenRadiusDuration).SetEase(Ease.OutBounce));

            // Изменяем smoothness от минимального до максимального и обратно к минимальному
            _sequence.Join(_material.DOFloat(MaxSmoothness, SoftnessParameter, TweenSmoothnessDuration))
                .Join(_material.DOFloat(MinSmoothness, SoftnessParameter, TweenSmoothnessDuration)
                    .SetDelay(TweenSmoothnessDuration).SetEase(Ease.OutBounce));

            //_sequence.SetEase(Ease.OutBounce);
            _sequence.Play();
        }

        public void OpenEyes()
        {
            if(_feature.isActive == false)
                throw new System.InvalidOperationException("Cannot open eyes when the feature is not active.");
        
            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            // Изменяем радиус от минимального до максимального значения
            _sequence.Join(_material.DOFloat(MinRadius, RadiusParameter, TweenRadiusDuration).SetEase(Ease.InBounce));

            // Изменяем smoothness от минимального до максимального и обратно к минимальному
            _sequence.Join(_material.DOFloat(MaxSmoothness, SoftnessParameter, TweenSmoothnessDuration))
                .Join(_material.DOFloat(MinSmoothness, SoftnessParameter, TweenSmoothnessDuration)
                    .SetDelay(TweenSmoothnessDuration).SetEase(Ease.InBounce));

            //_sequence.SetEase(Ease.InBounce);
            _sequence.OnComplete(() => Disable());
            _sequence.Play();
        }
    }
}