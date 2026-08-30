namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Tank : MonoBehaviour, IInteractable, IDamageable
    {
        [SerializeField]
        private float _invincibilityDuration = 0.3f;

        [Header("Debug")]
        [SerializeField]
        private Health _health = null;
        
        private TankStats _tankStats;
        
        private Timer _invincibilityTimer = null;
        
        private DestructibleStrategy[] _destructibleStrategies = null;
        private DamageUseCase _damageUseCase = null;
        
        private TankShootUseCase _tankShootUseCase = null;
        private TankMovementUseCase _tankMovementUseCase = null;
        
        private TankSkinRenderer _tankSkinRenderer = null;
        private Turret _turret = null;
        private Rigidbody2D _rigidbody2D = null;
        
        private bool _isInitialized = false;

        public Rigidbody2D Rigidbody2D => _rigidbody2D;
        public Vector2 Position => transform.position;
        public TankStats TankStats => _tankStats;
        public Turret Turret => _turret;

        public Health Health => _health;

        public bool CanNotTakeDamage => _invincibilityTimer.IsRunning;

        public void Init(TankStats tankStats)
        {
            _tankStats = tankStats;
            
            _health = new Health(_tankStats.Health);
            _invincibilityTimer = new Timer(this.Resolve<TimerUpdateHandler>()); 
            
            _tankShootUseCase = this.Resolve<TankShootUseCase>();
            _tankMovementUseCase = this.Resolve<TankMovementUseCase>();
            _damageUseCase = this.Resolve<DamageUseCase>();
            
            _tankSkinRenderer = GetComponentInChildren<TankSkinRenderer>(true);
            _turret = GetComponentInChildren<Turret>(true);
            
            _rigidbody2D = GetComponent<Rigidbody2D>();

            _tankSkinRenderer.Init(this);
            _turret.Init(_tankStats.TurretStats);

            _destructibleStrategies = GetComponentsInChildren<DestructibleStrategy>(true);
            foreach (var destructibleStrategy in _destructibleStrategies)
            {
                destructibleStrategy.Init(this);
            }
            
            _isInitialized = true;
        }
        
        public void Move(Vector2 input, float deltaTime)
        {
            _tankMovementUseCase.Execute(this, input, deltaTime);
        }
        
        public void ApplyRecoil(Vector2 force)
        {
            _rigidbody2D.AddForce(force, ForceMode2D.Impulse);
        }
        
        public void Rotate(Vector2 aimTarget, float deltaTime)
        {
            _turret.Rotate(aimTarget, deltaTime);
        }
        
        public void Shoot()
        {
            _tankShootUseCase.Execute(this);
        }
        
        public void Interact(Projectile projectile)
        {
            _damageUseCase.Execute(this, projectile.Damage);
        }
        
        public void TakeDamage()
        {
            _invincibilityTimer.Setup(_invincibilityDuration);
            _tankSkinRenderer.Flash(_invincibilityDuration);
        }

        public void Die()
        {
            _rigidbody2D.mass = 500f;
            _tankSkinRenderer.Dim();
            foreach (var destructibleStrategy in _destructibleStrategies)
            {
                destructibleStrategy.Destruct();
            }
        }
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(Tank)} not initialized", this);
            }
        }
#endif
    }
}