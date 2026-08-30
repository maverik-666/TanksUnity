namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class Turret : MonoBehaviour
    {
        [SerializeField]
        private Transform _projectilePoint;
        
        private TurretStats _turretStats;
        private TurretAimUseCase _turretAimUseCase = null;
        
        private BarrelSkinRenderer _barrelSkinRenderer = null;

        private Timer _cooldownTimer = null;
        
        private bool _isInitialized = false;
     
        public Vector2 Position => transform.position;
        public Quaternion Rotation => transform.rotation;

        public Vector3 ProjectilePosition => _projectilePoint.position;

        public TurretStats TurretStats => _turretStats; 
        
        public Timer CooldownTimer => _cooldownTimer;
        
        public bool CanShoot => CooldownTimer.IsFinished;
        
        public void Init(TurretStats turretStats)
        {
            _turretStats = turretStats;
            _turretAimUseCase = this.Resolve<TurretAimUseCase>();

            _cooldownTimer = new Timer(this.Resolve<TimerUpdateHandler>());

            _barrelSkinRenderer = GetComponentInChildren<BarrelSkinRenderer>(true);
            _barrelSkinRenderer.Init();
            
            _isInitialized = true;
        }

        public void Rotate(Vector2 aimTarget, float deltaTime)
        {
            _turretAimUseCase.Execute(this, aimTarget, deltaTime);
        }

        public void AnimateShoot()
        {
            _barrelSkinRenderer.AnimateShoot();
        }
        
#if UNITY_EDITOR  
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(Turret)} not initialized", this);
            }
        }
#endif
    }
}