namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class BattleDamageUseCase : MonoBehaviour, IDamageUseCase
    {
        private BattleCreator _battleCreator;
        private bool _isInitialized = false;

        public void Init(BattleCreator battleCreator)
        {
            _battleCreator = battleCreator;
            _isInitialized = true;
        }

        public void Execute(IDamageable damageable, int amount)
        {
            if (damageable.CanNotTakeDamage)
            {
                return;
            }
            
            damageable.Health.TakeDamage(amount);
            damageable.TakeDamage();

            if (damageable.IsDead)
            {
                damageable.Die();
            }

            _battleCreator.OnDamageTakenBy(damageable);
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