namespace Project
{
    using UnityEngine;
    
    public class CompositionRoot : Context
    {
        [SerializeField]
        private TankStatsSO _tankStats;
        
        private InputSystem _inputSystem = null;

        private PlayerInputHandler _playerInputHandler = null;
        private TimerUpdateHandler _timerUpdateHandler = null;
        private ProjectileUpdateHandler _projectileUpdateHandler = null;
        
        public void Awake()
        {
            var camera = GetComponentInChildren<Camera>(true);

            var player = GetComponentInChildren<Player>(true);
            player.Init();
            
            _inputSystem = GetComponentInChildren<InputSystem>(true);
            _playerInputHandler = GetComponentInChildren<PlayerInputHandler>(true);
            _timerUpdateHandler = GetComponentInChildren<TimerUpdateHandler>(true);

            _inputSystem.Init();
            _playerInputHandler.Init(_inputSystem, camera, player);
            _timerUpdateHandler.Init();
            
            var vfxFactory = GetComponentInChildren<OneShotVFXFactory>(true);
            vfxFactory.Init();
            
            var projectileRegistry = new Registry<Projectile>(50);
            _projectileUpdateHandler = GetComponentInChildren<ProjectileUpdateHandler>(true);
            var projectileFactory = GetComponentInChildren<ProjectileFactory>(true);
            var projectileCollisionUseCase = GetComponentInChildren<ProjectileCollisionUseCase>(true);

            
            _projectileUpdateHandler.Init(projectileRegistry);
            projectileFactory.Init(projectileRegistry);
            projectileCollisionUseCase.Init(projectileFactory, vfxFactory);
            
            GetComponentInChildren<TurretAimUseCase>(true).Init();
            GetComponentInChildren<TankMovementUseCase>(true).Init();
            GetComponentInChildren<TankShootUseCase>(true).Init();
            
            foreach (var tank in GetComponentsInChildren<Tank>(true))
            {
                tank.Init(_tankStats.TankStats);
            }
        }

        public void Update()
        {
            float deltaTime = Time.deltaTime;
            
            _inputSystem.UpdateTick(deltaTime);
            _playerInputHandler.UpdateTick(deltaTime);
            _timerUpdateHandler.UpdateTick(deltaTime);
            _projectileUpdateHandler.UpdateTick(deltaTime);
        }
    }
}