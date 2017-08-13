using System;
using System.Collections.Generic;

namespace GrimoireTD.TemporaryEffects
{
    public class TemporaryEffectsManager : IReadOnlyTemporaryEffectsManager
    {
        private List<TemporaryEffect> effectsList;

        public IReadOnlyList<IReadOnlyTemporaryEffect> EffectList
        {
            get
            {
                return effectsList;
            }
        }

        private Action<IReadOnlyTemporaryEffect> OnNewTemporaryEffectCallback;

        public TemporaryEffectsManager()
        {
            effectsList = new List<TemporaryEffect>();
        }

        public void ApplyEffect(object key, float magnitude, float duration, string effectName, Action onApplyCallback, Action onEndCallback)
        {
            TemporaryEffect currentEffect = effectsList.Find(x => x.Key == key);

            if (!ShouldApplyEffect(magnitude, duration, currentEffect))
            {
                return;
            }

            if (currentEffect != null)
            {
                currentEffect.EndNow();
            }

            onApplyCallback?.Invoke();

            TemporaryEffect newEffect = new TemporaryEffect(key, magnitude, duration, effectName, onEndCallback);

            effectsList.Add(newEffect);

            OnNewTemporaryEffectCallback?.Invoke(newEffect);

            newEffect.RegisterForOnEndCallback(() => OnEffectEnd(newEffect));
        }

        private bool ShouldApplyEffect(float newMagnitude, float newDuration, TemporaryEffect currentEffect)
        {
            if (currentEffect == null)
            {
                return true;
            }

            if (currentEffect.Magnitude < newMagnitude)
            {
                return true;
            }

            if (currentEffect.Magnitude == newMagnitude && currentEffect.TimeRemaining < newDuration)
            {
                return true;
            }

            return false;
        }

        private void OnEffectEnd(TemporaryEffect effect)
        {
            effectsList.Remove(effect);
        }

        public void RegisterForOnNewTemporaryEffectCallback(Action<IReadOnlyTemporaryEffect> callback)
        {
            OnNewTemporaryEffectCallback += callback;
        }

        public void DeregisterForOnNewTemporaryEffectCallback(Action<IReadOnlyTemporaryEffect> callback)
        {
            OnNewTemporaryEffectCallback -= callback;
        }
    }
}