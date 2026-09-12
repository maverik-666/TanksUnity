namespace Project
{
    using System;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(ParticleSystem))]
    public class OneShotVFX : MonoBehaviour
    { 
        private ParticleSystemEventHandler _particleSystemEventHandler = null;
        private ParticleSystem _particleSystem = null;
        
        private bool _isInitialized = false;
        
        public void Init()
        {
            _particleSystemEventHandler = GetComponentInChildren<ParticleSystemEventHandler>(true);
            _particleSystem = GetComponentInChildren<ParticleSystem>(true);
            
            _isInitialized = true;
        }

        public void Play(Action callback = null)
        {
            _particleSystemEventHandler.ParticleSystemStopped += OnParticleStopped;
            _particleSystem.Play(true);
            
            void OnParticleStopped()
            { 
                _particleSystemEventHandler.ParticleSystemStopped -= OnParticleStopped;
                callback?.Invoke();
            }
        }
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(OneShotVFX)} not initialized", this);
            }
        }
#endif
    }
}