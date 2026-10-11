using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Source.Scripts.DI.Services.Game.FSE
{
    public interface IHealthDamageEffect
    {
        public void Disable();

        public void Enable();

        public void PlayDamageEffect();
    }

    public class HealthVignetteEffect : IHealthDamageEffect
    {
        private const string RadiusParameter = "_Radius";
        private const float MinRadius = 0.2f;
        private const float MaxRadius = 1f;
        private const float TweenDuration = 0.5f;

        private readonly ScriptableRendererFeature _feature;
        private readonly Material _material;

        private Sequence _radiusSequence;

        public HealthVignetteEffect(ScriptableRendererFeature feature, Material material)
        {
            _feature = feature != null ? feature : throw new System.ArgumentNullException(nameof(feature));
            _material = material != null ? material : throw new System.ArgumentNullException(nameof(material));
            _feature.SetActive(false);
        }

        public void Enable()
        {
            _material.SetFloat(RadiusParameter, MaxRadius);
            _feature.SetActive(true);
        }

        public void Disable()
        {
            _material.SetFloat(RadiusParameter, MaxRadius);
            _feature.SetActive(false);
        }

        public void PlayDamageEffect()
        {
            _radiusSequence?.Kill();
            _radiusSequence = DOTween.Sequence();

            _radiusSequence.Append(_material.DOFloat(MinRadius, RadiusParameter, TweenDuration).SetEase(Ease.InSine));
            _radiusSequence.Append(_material.DOFloat(MaxRadius, RadiusParameter, TweenDuration).SetEase(Ease.OutSine));
            _radiusSequence.Play();
        }
    }
}