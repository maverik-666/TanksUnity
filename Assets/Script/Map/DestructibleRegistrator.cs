namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class DestructibleRegistrator : MonoBehaviour
    {
        private IndexedRegistry<IDamageable> _damageableRegistry = null;
        private int _idCounter = 0;
        
        private bool _isInitialized = false;

        public void Init(IndexedRegistry<IDamageable> damageableRegistry)
        {
            _damageableRegistry = damageableRegistry;
            _isInitialized = true;
        }

        public void Register(IDamageable damageable)
        {
            _damageableRegistry.Add(_idCounter, damageable);
            _idCounter++;
        }

        public void ResetCounter()
        {
            _idCounter = 0;
        }

        public void Unregister(IDamageable damageable)
        {
            _damageableRegistry.Remove(damageable);
        }

        public bool HasDestructible(IDamageable damageable)
        {
            return _damageableRegistry.HasItem(damageable);
        }
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(DestructibleRegistrator)} not initialized", this);
            }
        }
#endif
    }
}