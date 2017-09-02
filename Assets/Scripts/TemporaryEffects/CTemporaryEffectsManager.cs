using System;
using System.Collections.Generic;

namespace GrimoireTD.TemporaryEffects
{
    public class CTemporaryEffectsManager : ITemporaryEffectsManager
    {
        private List<ITemporaryEffect> effectsList;

        public IReadOnlyList<IReadOnlyTemporaryEffect> EffectList
        {
            get
            {
                return effectsList;
            }
        }

        public event EventHandler<EAOnNewTemporaryEffect> OnNewTemporaryEffect;

        public CTemporaryEffectsManager()
        {
            effectsList = new List<ITemporaryEffect>();
        }

        public void ApplyEffect(object key, float magnitude, float duration, string effectName, Action onApplyCallback, EventHandler<EAOnTemporaryEffectEnd> onEndEvent)
        {
            ITemporaryEffect currentEffect = effectsList.Find(x => x.Key == key);

            if (!ShouldApplyEffect(magnitude, duration, currentEffect))
            {
                return;
            }

            if (currentEffect != null)
            {
                currentEffect.EndNow();
            }

            onApplyCallback?.Invoke();

            ITemporaryEffect newEffect = new CTemporaryEffect(key, magnitude, duration, effectName, onEndEvent);

            effectsList.Add(newEffect);

            OnNewTemporaryEffect?.Invoke(this, new EAOnNewTemporaryEffect(newEffect));

            newEffect.OnTemporaryEffectEnd += (object sender, EAOnTemporaryEffectEnd args) => OnEffectEnd(newEffect);
        }

        private bool ShouldApplyEffect(float newMagnitude, float newDuration, ITemporaryEffect currentEffect)
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

        private void OnEffectEnd(ITemporaryEffect effect)
        {
            effectsList.Remove(effect);
        }
    }
}