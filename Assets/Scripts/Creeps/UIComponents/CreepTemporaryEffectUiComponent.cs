using System;
using UnityEngine;
using UnityEngine.UI;
using GrimoireTD.TemporaryEffects;

namespace GrimoireTD.Creeps
{
    public class CreepTemporaryEffectUiComponent : MonoBehaviour
    {
        private IReadOnlyTemporaryEffect temporaryEffect;

        private Action OnDestroyCallback;

        [SerializeField]
        private Text ownText;
        [SerializeField]
        private Slider ownSlider;

        public CreepTemporaryEffectUiComponent SetUp(IReadOnlyTemporaryEffect temporaryEffect)
        {
            this.temporaryEffect = temporaryEffect;

            ownSlider.maxValue = temporaryEffect.Duration;

            temporaryEffect.OnTemporaryEffectEnd += OnEffectEnd;

            return this;
        }

        private void Update()
        {
            SetValues();
        }

        private void SetValues()
        {
            ownText.text = temporaryEffect.EffectName + " - " + temporaryEffect.Magnitude + " - " + temporaryEffect.TimeRemaining.ToString("0.0");
            ownSlider.value = Mathf.Max(temporaryEffect.Duration - temporaryEffect.Elapsed, 0f);
        }

        private void OnDestroy()
        {
            OnDestroyCallback?.Invoke();

            temporaryEffect.OnTemporaryEffectEnd -= OnEffectEnd;
        }

        private void OnEffectEnd(object sender, EAOnTemporaryEffectEnd args)
        {
            Destroy(gameObject);
        }

        public void RegiterForOnDestroyCallback(Action callback)
        {
            OnDestroyCallback += callback;
        }

        public void DeregisterForOnDestroyCallback(Action callback)
        {
            OnDestroyCallback -= callback;
        }
    }
}