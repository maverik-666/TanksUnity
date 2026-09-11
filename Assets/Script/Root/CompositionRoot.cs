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
            Bind(updateHandlerRegistry);
            
            var ownerPlayerRegistry = new OwnerPlayerRegistry();
            Bind(ownerPlayerRegistry);
            
            var camera = GetComponentInChildren<Camera>(true);
            var cameraFollowHandler = GetComponentInChildren<CameraFollowHandler>(true);
            cameraFollowHandler.Init(camera, ownerPlayerRegistry);
            updateHandlerRegistry.Add(cameraFollowHandler);

            var cameraBoundsUpdateHandler = GetComponentInChildren<CameraBoundsUpdateHandler>(true);
            cameraBoundsUpdateHandler.Init(camera);
            updateHandlerRegistry.Add(cameraBoundsUpdateHandler);
            
            GetComponentInChildren<TankFactory>(true).Init();
            
            _inputSystem = GetComponentInChildren<InputSystem>(true);
            _inputSystem.Init();

            var ownerPlayerInputHandler = GetComponentInChildren<OwnerPlayerInputHandler>(true);
            ownerPlayerInputHandler.Init(_inputSystem, camera, ownerPlayerRegistry);
            updateHandlerRegistry.Add(ownerPlayerInputHandler);
            
            var timerUpdateHandler = GetComponentInChildren<TimerUpdateHandler>(true);
            timerUpdateHandler.Init();
            updateHandlerRegistry.Add(timerUpdateHandler);
            
            GetComponentInChildren<DamageUseCase>(true).Init();
            
            var vfxFactory = GetComponentInChildren<OneShotVFXFactory>(true);
            vfxFactory.Init();
            
            var projectileRegistry = new Registry<Projectile>(50);

            var projectileUpdateHandler = GetComponentInChildren<ProjectileUpdateHandler>(true);
            projectileUpdateHandler.Init(projectileRegistry);
            updateHandlerRegistry.Add(projectileUpdateHandler);
            
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
            
            mapGenerator.Init(cameraBoundsUpdateHandler, boxFactory, gasolineFactory, bushFactory);
            mapGenerator.Generate(0);
            
            GetComponentInChildren<GameUpdater>(true).Init();
        }
    }
}