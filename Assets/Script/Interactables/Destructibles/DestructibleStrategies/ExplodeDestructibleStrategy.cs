namespace Project
{
    using UnityEngine;

    [RequireComponent(typeof(Transform))]
    public class ExplodeDestructibleStrategy : DestructibleStrategy
    {
        [SerializeField]
        private int _damage = 50;
        
        [SerializeField]
        private float _power = 50;
        
        [SerializeField]
        private float _radius = 2;

        private DamageUseCase _damageUseCase = null;
        private IDamageable _damageable = null;
        
        private bool _isInitialized = false;

        public override void Init(IDamageable damageable)
        {
            _damageUseCase = this.Resolve<DamageUseCase>();
            _damageable = damageable;
            _isInitialized = true;
        }

        public override void Destruct()
        {
            var colliders = Physics2D.OverlapCircleAll(_damageable.Position, _radius);

            foreach (var hit in colliders) 
            { 
                if (!hit.TryGetComponent<Rigidbody2D>(out var rb))
                {
                    continue;
                }

                if (hit.TryGetComponent<IDamageable>(out var damageable))
                {
                    _damageUseCase.Execute(damageable, _damage);
                }
                
                var direction = rb.position - _damageable.Position;
                float distance = direction.magnitude;

                if (distance == 0)
                {
                    distance = 0.05f;
                }
        
                var force = direction.normalized * (_power / distance);
                rb.AddForce(force, ForceMode2D.Impulse);
            }
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(ExplodeDestructibleStrategy)} not initialized", this);
            }
        }
#endif
    }
}