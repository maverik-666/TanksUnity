namespace Project
{
    using UnityEngine;
    
    public class NetworkCompositionRoot : Context
    {
        private static Context _instance = null;
        
        public static Context Instance => _instance;
        
        public void Awake()
        {
            _instance = this;
            
            var updateHandlerRegistry = new Registry<IUpdateHandler>(20);
            Bind(updateHandlerRegistry);
            
            var ownerPlayerRegistry = new OwnerPlayerRegistry();
            Bind(ownerPlayerRegistry);
                
            var allTankRegistry = new AllTankRegistry();
            Bind(allTankRegistry);
            
            var damageableRegistry = new IndexedRegistry<IDamageable>(500);
            Bind(damageableRegistry);
            
            var interactableRegistry = new IndexedRegistry<IInteractable>(500);
            Bind(interactableRegistry);

            var playerRegistry = new PlayerRegistry(8);
            Bind(playerRegistry);

            var activeMapRegistry = new ActiveMapRegistry();
            Bind(activeMapRegistry);
            
            var camera = GetComponentInChildren<Camera>(true);
            var cameraFollowHandler = GetComponentInChildren<CameraFollower>(true);
            cameraFollowHandler.Init(camera, ownerPlayerRegistry);

            var cameraBoundsUpdateHandler = GetComponentInChildren<CameraBoundsUpdateHandler>(true);
            cameraBoundsUpdateHandler.Init(camera);
            updateHandlerRegistry.Add(cameraBoundsUpdateHandler);
            
            GetComponentInChildren<PlayerHealthBar>(true).Init();
            
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
            
            var projectileFactory = GetComponentInChildren<NetworkProjectileFactory>(true);
            var projectileCollisionUseCase = GetComponentInChildren<NetworkProjectileInteractUseCase>(true);

            projectileFactory.Init(projectileRegistry, indexedProjectileRegistry, allTankRegistry);
            projectileCollisionUseCase.Init(indexedProjectileRegistry, projectileFactory, vfxFactory);
            
            GetComponentInChildren<TurretAimUseCase>(true).Init();
            GetComponentInChildren<TankMovementUseCase>(true).Init();
            GetComponentInChildren<TankShootUseCase>(true).Init(projectileFactory);

            var destructibleRegistrator = GetComponentInChildren<DestructibleRegistrator>(true);
            
            var boxFactory = GetComponentInChildren<BoxFactory>(true);
            var gasolineFactory = GetComponentInChildren<GasolineFactory>(true);
            var bushFactory = GetComponentInChildren<BushFactory>(true);
            
            destructibleRegistrator.Init(damageableRegistry);
            
            boxFactory.Init(destructibleRegistrator);
            gasolineFactory.Init(destructibleRegistrator);
            bushFactory.Init();

            var mapConfigProvider = GetComponentInChildren<BattleMapConfigProvider>(true);
            var mapGenerator = GetComponentInChildren<BattleMapGenerator>(true);

            mapGenerator.Init(boxFactory, gasolineFactory, bushFactory);
            mapConfigProvider.Init(playerRegistry);
            
            var presetMap = GetComponentInChildren<PresetMap>(true);
            presetMap.Init();
            activeMapRegistry.Set(presetMap);
            
            var battleCreator = GetComponentInChildren<BattleCreator>(true);
            battleCreator.Init(cameraBoundsUpdateHandler, playerRegistry, presetMap, destructibleRegistrator, mapGenerator, mapConfigProvider);
            
            GetComponentInChildren<GameUpdater>(true).Init(updateHandlerRegistry);

            if (SessionConnector.Instance == null)
            {
                SessionConnector.HostInstant();
            }
            else
            {
                _ = SessionConnector.Instance.FulfillRequest();
            }
        }
    }
}