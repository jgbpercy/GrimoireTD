using System;
using UnityEngine;
using GrimoireTD.Technical;

namespace GrimoireTD.TemporaryEffects
{
    public class CTemporaryEffect : ITemporaryEffect, IFrameUpdatee
    {
        public object Key { get; }

        public float Magnitude { get; }

        public float Duration { get; }
        public float Elapsed { get; private set; }

        public string EffectName { get; }

        public event EventHandler<EAOnTemporaryEffectEnd> OnTemporaryEffectEnd;

        public float TimeRemaining
        {
            get
            {
                return Duration - Elapsed;
            }
        }
        
        public CTemporaryEffect(object key, float magnitude, float duration, string effectName, EventHandler<EAOnTemporaryEffectEnd> onEndEvent)
        {
            Key = key;
            Magnitude = magnitude;
            Duration = duration;
            Elapsed = 0f;
            EffectName = effectName;
            OnTemporaryEffectEnd = onEndEvent;

            ModelObjectFrameUpdater.Instance.RegisterAsModelObjectFrameUpdatee(this);
        }

        public void ModelObjectFrameUpdate()
        {
            Elapsed += Time.deltaTime;

            if (Elapsed > Duration)
            {
                OnTemporaryEffectEnd?.Invoke(this, new EAOnTemporaryEffectEnd(this));
            }
        }
        
        public void EndNow()
        {
            OnTemporaryEffectEnd?.Invoke(this, new EAOnTemporaryEffectEnd(this));
        }
    }
}