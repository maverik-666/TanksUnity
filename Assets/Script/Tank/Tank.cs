namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Tank : MonoBehaviour
    {
        private TankStats _tankStats;
        
        private TankShootUseCase _tankShootUseCase = null;
        private TankMovementUseCase _tankMovementUseCase = null;
        
        private TankSkinRenderer _tankSkinRenderer = null;
        private Turret _turret = null;
        private Rigidbody2D _rigidbody2D = null;
        
        private bool _isInitialized = false;

        public Rigidbody2D Rigidbody2D => _rigidbody2D;
        public TankStats TankStats => _tankStats;
        public Turret Turret => _turret;

        public void Init(TankStats tankStats)
        {
            _tankStats = tankStats;
            
            _tankShootUseCase = this.Resolve<TankShootUseCase>();
            _tankMovementUseCase = this.Resolve<TankMovementUseCase>();
            
            _tankSkinRenderer = GetComponentInChildren<TankSkinRenderer>(true);
            _turret = GetComponentInChildren<Turret>(true);
            
            _rigidbody2D = GetComponent<Rigidbody2D>();

            _tankSkinRenderer.Init(this);
            _turret.Init(_tankStats.TurretStats);
            
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