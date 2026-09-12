namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 2;
        
        [SerializeField]
        private float _radius = 0.05f;

        [SerializeField]
        private LayerMask _layerMask;

        private IProjectileInteractUseCase _projectileInteractUseCase = null;
        
        private int _damage = 0;
        private Tank _owner = null;
        private Vector2 _direction = Vector2.zero;
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
            
            _projectileInteractUseCase = this.Resolve<IProjectileInteractUseCase>();
            
            _isInitialized = true;
        }

        public void UpdateTick(float deltaTime)
        {
            var lastPos = Position;
            var newPos = transform.position = lastPos + (Vector3) _direction * (_speed * deltaTime);
            
            var hit = Physics2D.CircleCast(lastPos, _radius, newPos-lastPos, Vector3.Distance(newPos, lastPos), _layerMask);

            if (hit.collider)
            { 
                _projectileInteractUseCase.Resolve(this, hit.collider);
            }
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