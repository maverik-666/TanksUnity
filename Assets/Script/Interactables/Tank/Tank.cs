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
        private IDamageUseCase _damageUseCase = null;
        
        private TankShootUseCase _tankShootUseCase = null;
        private TankMovementUseCase _tankMovementUseCase = null;
        
        private TankSkinRenderer _tankSkinRenderer = null;
        private Turret _turret = null;
        private Rigidbody2D _rigidbody2D = null;
        private IPlayer _player = null;

        private bool _isInvincible = false;
        private bool _isInitialized = false;

        public Rigidbody2D Rigidbody2D => _rigidbody2D;
        public Vector2 Position => transform.position;
        public Quaternion Rotation => transform.rotation;
        public Vector2 LinearVelocity => _rigidbody2D.linearVelocity;
        public float AngularVelocity => _rigidbody2D.angularVelocity;
        
        public TankStats TankStats => _tankStats;
        public Turret Turret => _turret;
        
        public IPlayer Player => _player; 
        
        public bool CanShoot => Turret.CanShoot;
        
        public Health Health => _health;
        
        public bool IsDead => _health.IsDead;

        public bool CanNotTakeDamage => _invincibilityTimer.IsRunning || _isInvincible;

        public void Init(IPlayer player, TankStats tankStats)
        {
            _player = player;
            _tankStats = tankStats;
            
            _health = new Health(_tankStats.Health);
            _invincibilityTimer = new Timer(this.Resolve<TimerUpdateHandler>()); 
            
            _tankShootUseCase = this.Resolve<TankShootUseCase>();
            _tankMovementUseCase = this.Resolve<TankMovementUseCase>();
            _damageUseCase = this.Resolve<IDamageUseCase>();
            
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

        public void SetInvincibility(bool isInvincible)
        {
            _isInvincible = isInvincible;
        }
        
        public void Move(Vector2 input, float deltaTime)
        {
            _tankMovementUseCase.Execute(this, input, deltaTime);
            _tankSkinRenderer.UpdatePosition();
        }

        public void UpdatePosition()
        {
            _tankSkinRenderer.UpdatePosition();
        }
        
        public void ApplyRecoil(Vector2 force)
        {
            _rigidbody2D.AddForce(force, ForceMode2D.Impulse);
        }
        
        public void Rotate(Vector2 aimTarget, float deltaTime)
        {
            _turret.Rotate(aimTarget, deltaTime);
        }
        
        public void Shoot(bool isInstant)
        {
            _tankShootUseCase.Execute(this, isInstant);
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
            _rigidbody2D.mass = 500000f;
            _tankSkinRenderer.Dim();
            foreach (var destructibleStrategy in _destructibleStrategies)
            {
                destructibleStrategy.Destruct();
            }
        }

        public void Revive()
        {
            _health.SetHealth(_health.Max);

            _rigidbody2D.mass = 200f;
            _tankSkinRenderer.Brighten();
            _tankSkinRenderer.Flash(_invincibilityDuration);
        }

        public void Teleport()
        {
            _tankSkinRenderer.Teleport();
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