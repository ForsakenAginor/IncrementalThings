using Sirenix.OdinInspector;
using Source.Scripts.DI.Services.Game.FSE;
using UnityEngine;
using Zenject;

namespace Source.Scripts.Cheats
{
    public class CheatsPanel : MonoBehaviour
    {
#if UNITY_EDITOR
        private IHealthDamageEffect _healthVignette;
        private INoiceVignetteEffect _noiceEffect;

        [Inject]
        public void Construct(IHealthDamageEffect healthVignette, INoiceVignetteEffect noiceEffect)
        {
            _healthVignette = healthVignette;
            _noiceEffect = noiceEffect;
        }


        [Button]
        private void PlayDamageEffect()
        {
            _healthVignette.PlayDamageEffect();
        }

        [Button]
        private void PlayDamageNoiceEffect(float strength)
        {
            _noiceEffect.SetEffectStrength(strength);
        }
#endif
    }
}