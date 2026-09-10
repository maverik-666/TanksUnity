namespace Project
{
    using System.Collections.Generic;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class TimerUpdateHandler : MonoBehaviour, IUpdateHandler
    {
        private List<Timer> _timers = new List<Timer>();
        private bool _isInitialized = false;
        
        public void Init()
        {
            _isInitialized = true;
        }

        public void Register(Timer timer)
        {
            _timers.Add(timer);
        }
        
        public void UpdateTick(float deltaTime)
        {
            for (int i = _timers.Count - 1; i >= 0; i--)
            {
                var timer = _timers[i];
                timer.Tick(deltaTime);

                if (timer.IsFinished)
                {
                    _timers.Remove(timer);
                }
            }
        }
        
#if UNITY_EDITOR  
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(TimerUpdateHandler)} not initialized", this);
            }
        }
#endif
    }
}