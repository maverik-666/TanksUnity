namespace Project
{
    using UnityEngine;
    
    public class CompositionRoot : Context
    {
        private static CompositionRoot _instance = null;
        
        private InputSystem _inputSystem = null;

        public static CompositionRoot Instance => _instance;
        
        public void Awake()
        {
            _instance = this;

            var updateHandlerRegistry = new Registry<IUpdateHandler>(20);
            updateHandlerRegistry.AddRange(GetComponentsInChildren<IUpdateHandler>(true));
            Bind(updateHandlerRegistry);
            
            var ownerPlayerRegistry = new OwnerPlayerRegistry();
            Bind(ownerPlayerRegistry);
            
            var camera = GetComponentInChildren<Camera>(true);
            GetComponentInChildren<CameraFollowHandler>(true).Init(camera, ownerPlayerRegistry);

            GetComponentInChildren<TankFactory>(true).Init();
            
            _inputSystem = GetComponentInChildren<InputSystem>(true);
            _inputSystem.Init(); 
            
            GetComponentInChildren<OwnerPlayerInputHandler>(true).Init(_inputSystem, camera, ownerPlayerRegistry); 
            GetComponentInChildren<TimerUpdateHandler>(true).Init();
            
            GetComponentInChildren<DamageUseCase>(true).Init();
            
            var vfxFactory = GetComponentInChildren<OneShotVFXFactory>(true);
            vfxFactory.Init();
            
            var projectileRegistry = new Registry<Projectile>(50);
            
            GetComponentInChildren<ProjectileUpdateHandler>(true).Init(projectileRegistry);

            var projectileFactory = GetComponentInChildren<ProjectileFactory>(true);
            var projectileCollisionUseCase = GetComponentInChildren<ProjectileInteractUseCase>(true);
            
            projectileFactory.Init(projectileRegistry);
            projectileCollisionUseCase.Init(projectileFactory, vfxFactory);
            
            GetComponentInChildren<TurretAimUseCase>(true).Init();
            GetComponentInChildren<TankMovementUseCase>(true).Init();
            GetComponentInChildren<TankShootUseCase>(true).Init();

            var mapGenerator = GetComponentInChildren<MapGenerator>(true);
            
            var boxFactory = GetComponentInChildren<BoxFactory>(true);
            var gasolineFactory = GetComponentInChildren<GasolineFactory>(true);
            var bushFactory = GetComponentInChildren<BushFactory>(true);
            
            boxFactory.Init();
            gasolineFactory.Init();
            bushFactory.Init();
            
            mapGenerator.Init(boxFactory, gasolineFactory, bushFactory);
            mapGenerator.Generate(0);
            
            GetComponentInChildren<GameUpdater>(true).Init();
        }
    }
}