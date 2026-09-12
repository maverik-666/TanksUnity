namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class PlayerHealthBar : MonoBehaviour
    {
        private SmoothSlider _smoothSlider = null;
        private Health _health;
        private bool _isInitialized = false;

        public void Init()
        {
            _smoothSlider = GetComponentInChildren<SmoothSlider>(true);
            _isInitialized = true;
        }

        public void SetTarget(Health health)
        {
            _health = health;
            _smoothSlider.SetForcedPosition((float)_health.Current / _health.Max);
            _health.Changed += OnHealthChange;
        }

        private void OnHealthChange()
        {
            _smoothSlider.SetPosition((float)_health.Current / _health.Max);
        }
        
        private void OnDestroy()
        {
            if (_health != null)
            {
                _health.Changed -= OnHealthChange;
            }
        }
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(PlayerHealthBar)} not initialized", this);
            }
        }
#endif
    }
}