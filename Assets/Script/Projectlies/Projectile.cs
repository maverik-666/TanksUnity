namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 2;

        private ProjectileInteractUseCase _projectileInteractUseCase = null;
        
        private int _damage = 0;
        private Tank _owner = null;
        private Vector2 _direction = Vector2.zero;
        private Rigidbody2D _rigidbody2D = null;
        private bool _isInitialized = false;
        
        public Vector3 Position => transform.position;
        
        public int Damage => _damage;
        
        public Vector2 Direction => _direction;
        
        public Tank Owner => _owner;
        
        public void Init(Tank owner, int damage, Vector2 direction)
        {
            _damage = damage;
            _owner = owner;
            _direction = direction;
            
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _projectileInteractUseCase = this.Resolve<ProjectileInteractUseCase>();
            
            _isInitialized = true;
        }

        public void UpdateTick(float deltaTime)
        {
            _rigidbody2D.linearVelocity = _direction * (_speed * deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _projectileInteractUseCase.Resolve(this, other);
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(Projectile)} not initialized", this);
            }
        }
#endif
    }
}