namespace Project
{
    using UnityEngine;
    
    public class CompositionRoot : Context
    {
        private static CompositionRoot _instance = null;
        
        private InputSystem _inputSystem = null;

        private OwnerPlayerInputHandler _ownerPlayerInputHandler = null;
        private TimerUpdateHandler _timerUpdateHandler = null;
        private ProjectileUpdateHandler _projectileUpdateHandler = null;
        

        public static CompositionRoot Instance => _instance;
        
        public void Awake()
        {
            _instance = this;
            
            var ownerPlayer = new OwnerPlayerRegistry();
            Bind(ownerPlayer);
            
            var camera = GetComponentInChildren<Camera>(true);

            var tankFactory = GetComponentInChildren<TankFactory>(true);
            tankFactory.Init();
            
            _inputSystem = GetComponentInChildren<InputSystem>(true);
            _ownerPlayerInputHandler = GetComponentInChildren<OwnerPlayerInputHandler>(true);
            _timerUpdateHandler = GetComponentInChildren<TimerUpdateHandler>(true);
            
            _inputSystem.Init(); 
            _ownerPlayerInputHandler.Init(_inputSystem, camera, ownerPlayer);
            _timerUpdateHandler.Init();
            
            var vfxFactory = GetComponentInChildren<OneShotVFXFactory>(true);
            vfxFactory.Init();
            
            var projectileRegistry = new Registry<Projectile>(50);
            _projectileUpdateHandler = GetComponentInChildren<ProjectileUpdateHandler>(true);
            var projectileFactory = GetComponentInChildren<ProjectileFactory>(true);
            var projectileCollisionUseCase = GetComponentInChildren<ProjectileInteractUseCase>(true);
            
            var damageUseCase = GetComponentInChildren<DamageUseCase>(true);
            damageUseCase.Init();
            
            _projectileUpdateHandler.Init(projectileRegistry);
            projectileFactory.Init(projectileRegistry);
            projectileCollisionUseCase.Init(projectileFactory, vfxFactory);
            
            GetComponentInChildren<TurretAimUseCase>(true).Init();
            GetComponentInChildren<TankMovementUseCase>(true).Init();
            GetComponentInChildren<TankShootUseCase>(true).Init();
            
            foreach (var destructible in GetComponentsInChildren<Destructible>(true))
            {
                destructible.Init();
            }
        }

        public void Update()
        {
            float deltaTime = Time.deltaTime;
            
            _inputSystem.UpdateTick(deltaTime);
            _ownerPlayerInputHandler.UpdateTick(deltaTime);
            _timerUpdateHandler.UpdateTick(deltaTime);
            _projectileUpdateHandler.UpdateTick(deltaTime);
        }
    }
}