namespace Project
{
    using Unity.Netcode;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class Destructible : MonoBehaviour, IInteractable, IDamageable
    {       
        [SerializeField]
        private int _initialHealth = 100;

        [SerializeField]
        private float _invincibilityDuration = 0.3f;
        
        [Header("Debug")]
        [SerializeField]
        private Health _health = null;

        private Timer _invincibilityTimer = null;
        
        private InteractableRegistrator _interactableRegistrator;
        private DestructibleStrategy[] _destructibleStrategies = null;
        private IDamageUseCase _damageUseCase = null;
        private DestructibleSkinRenderer _skinRenderer = null;
        private bool _isInitialized = false;
        
        public Health Health => _health;
  
        public Vector2 Position => transform.position;

        public bool CanNotTakeDamage => _invincibilityTimer.IsRunning;
        
        public void Init(InteractableRegistrator interactableRegistrator)
        {
            _interactableRegistrator = interactableRegistrator;
            
            _health = new Health(_initialHealth);
            _invincibilityTimer = new Timer(this.Resolve<TimerUpdateHandler>());
            
            _skinRenderer = GetComponentInChildren<DestructibleSkinRenderer>(true);
            _skinRenderer.Init();
            
            _damageUseCase = this.Resolve<IDamageUseCase>();
            _destructibleStrategies = GetComponentsInChildren<DestructibleStrategy>(true);
            
            foreach (var destructibleStrategy in _destructibleStrategies)
            {
                destructibleStrategy.Init(this);
            }
            
            interactableRegistrator.Register(this);
            _isInitialized = true;
        }
        
        public void Interact(Projectile projectile)
        {
            _damageUseCase.Execute(this, projectile.Damage);
        }
        
        public void TakeDamage()
        {
            _invincibilityTimer.Setup(_invincibilityDuration);
            _skinRenderer.Flash(_invincibilityDuration);
        }
        
        public void Die()
        {
            foreach (var destructibleStrategy in _destructibleStrategies)
            {
                destructibleStrategy.Destruct();
            }
            
            _interactableRegistrator.Unregister(this);
            Destroy(gameObject);
        }
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(Destructible)} not initialized", this);
            }
        }
#endif
    }
}