namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class SettingButton : MonoBehaviour, IInteractable
    {
        private CounterSetting _setting = null;
        private bool _isInitialized = false;

        public void Init(CounterSetting setting)
        {
            _setting = setting;
            _isInitialized = true;
        }
        
        public void Interact(Projectile projectile)
        {
            _setting.OnInteractWith(this);
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(SettingButton)} not initialized", this);
            }
        }
#endif
    }
}