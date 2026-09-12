namespace Project
{
    using UnityEngine;
    
    public class CompositionRoot : Context
    {
        private static CompositionRoot _instance = null;
        
        public static CompositionRoot Instance => _instance;
        
        public void Awake()
        {
            _instance = this;

            var updateHandlerRegistry = new Registry<IUpdateHandler>(20);
            Bind(updateHandlerRegistry);
            
            var ownerPlayerRegistry = new OwnerPlayerRegistry();
            Bind(ownerPlayerRegistry);
                
            var allTankRegistry = new AllTankRegistry();
            Bind(allTankRegistry);

            var spawnPointRegistry = new Registry<SpawnPoint>(20);
            Bind(spawnPointRegistry);
            
            var damageableRegistry = new IndexedRegistry<IDamageable>(500);
            Bind(damageableRegistry);
            
            var camera = GetComponentInChildren<Camera>(true);
            var cameraFollowHandler = GetComponentInChildren<CameraFollowHandler>(true);
            cameraFollowHandler.Init(camera, ownerPlayerRegistry);
            updateHandlerRegistry.Add(cameraFollowHandler);

            var cameraBoundsUpdateHandler = GetComponentInChildren<CameraBoundsUpdateHandler>(true);
            cameraBoundsUpdateHandler.Init(camera);
            updateHandlerRegistry.Add(cameraBoundsUpdateHandler);
            
            GetComponentInChildren<TankFactory>(true).Init();
            
            var inputUpdateHandler = GetComponentInChildren<InputUpdateHandler>(true);
            inputUpdateHandler.Init();
            updateHandlerRegistry.Add(inputUpdateHandler);

            var ownerPlayerInputHandler = GetComponentInChildren<OwnerPlayerInputHandler>(true);
            ownerPlayerInputHandler.Init(inputUpdateHandler, camera, ownerPlayerRegistry);
            updateHandlerRegistry.Add(ownerPlayerInputHandler);
            
            var timerUpdateHandler = GetComponentInChildren<TimerUpdateHandler>(true);
            timerUpdateHandler.Init();
            updateHandlerRegistry.Add(timerUpdateHandler);
            
            GetComponentInChildren<NetworkDamageUseCase>(true).Init(damageableRegistry);
            
            var vfxFactory = GetComponentInChildren<OneShotVFXFactory>(true);
            vfxFactory.Init();
            
            var projectileRegistry = new Registry<Projectile>(50);
            var indexedProjectileRegistry = new IndexedRegistry<Projectile>(50);

            var projectileUpdateHandler = GetComponentInChildren<ProjectileUpdateHandler>(true);
            projectileUpdateHandler.Init(projectileRegistry);
            updateHandlerRegistry.Add(projectileUpdateHandler);
            
            var projectileFactory = GetComponentInChildren<ProjectileFactory>(true);
            var projectileCollisionUseCase = GetComponentInChildren<NetworkProjectileInteractUseCase>(true);

            projectileFactory.Init(projectileRegistry, indexedProjectileRegistry, allTankRegistry);
            projectileCollisionUseCase.Init(indexedProjectileRegistry, projectileFactory, vfxFactory);
            
            GetComponentInChildren<TurretAimUseCase>(true).Init();
            GetComponentInChildren<TankMovementUseCase>(true).Init();
            GetComponentInChildren<TankShootUseCase>(true).Init();

            var mapGenerator = GetComponentInChildren<MapGenerator>(true);

            var destructibleRegistrator = GetComponentInChildren<DestructibleRegistrator>(true);
            
            var boxFactory = GetComponentInChildren<BoxFactory>(true);
            var gasolineFactory = GetComponentInChildren<GasolineFactory>(true);
            var bushFactory = GetComponentInChildren<BushFactory>(true);
            
            destructibleRegistrator.Init(damageableRegistry);
            
            boxFactory.Init(destructibleRegistrator);
            gasolineFactory.Init(destructibleRegistrator);
            bushFactory.Init();
            
            mapGenerator.Init(cameraBoundsUpdateHandler, boxFactory, gasolineFactory, bushFactory);
            mapGenerator.Generate(0);
            
            GetComponentInChildren<GameUpdater>(true).Init(updateHandlerRegistry);
        }
    }
}