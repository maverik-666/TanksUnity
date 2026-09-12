namespace Project
{
    using Unity.Netcode;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class NetworkDamageUseCase : NetworkBehaviour, IDamageUseCase
    {
        private IndexedRegistry<IDamageable> _damageableRegistry = null;
        private bool _isInitialized = false;

        public void Init(IndexedRegistry<IDamageable> damageableRegistry)
        {
            _damageableRegistry = damageableRegistry;
            _isInitialized = true;
        }

        public void Execute(IDamageable damageable, int amount)
        {
            if (!IsServer)
            {
                return;
            }
            
            if (damageable.CanNotTakeDamage)
            {
                return;
            }

            DamageRpc(_damageableRegistry.GetIdBy(damageable), amount);
            DealDamage(damageable, amount);
        }

        [Rpc(SendTo.NotServer)]
        private void DamageRpc(int id, int amount)
        {
            DealDamage(_damageableRegistry.GetItemBy(id), amount);
        }

        private void DealDamage(IDamageable damageable, int amount)
        {
            damageable.Health.TakeDamage(amount);
            damageable.TakeDamage();

            if (damageable.IsDead)
            {
                _damageableRegistry.Remove(damageable);
                damageable.Die();
            }
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(DamageUseCase)} not initialized", this);
            }
        }
#endif
    }
}