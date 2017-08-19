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

        private Action OnEndCallback;

        public float TimeRemaining
        {
            get
            {
                return Duration - Elapsed;
            }
        }
        
        public CTemporaryEffect(object key, float magnitude, float duration, string effectName, Action onEndCallback)
        {
            Key = key;
            Magnitude = magnitude;
            Duration = duration;
            Elapsed = 0f;
            EffectName = effectName;
            OnEndCallback = onEndCallback;

            ModelObjectFrameUpdater.Instance.RegisterAsModelObjectFrameUpdatee(this);
        }

        public void ModelObjectFrameUpdate()
        {
            Elapsed += Time.deltaTime;

            if (Elapsed > Duration)
            {
                OnEndCallback?.Invoke();
            }
        }
        
        public void EndNow()
        {
            OnEndCallback?.Invoke();
        }

        public void RegisterForOnEndCallback(Action callback)
        {
            OnEndCallback += callback;
        }

        public void DeregisterForOnEndCallback(Action callback)
        {
            OnEndCallback -= callback;
        }
    }
}