namespace Project
{
    using System;
    using UnityEngine;
    
    public class MainMenuCompositionRoot : Context
    {
        private void Awake()
        {
            InitSelf(this);
            
            var updateHandlerRegistry = new Registry<IUpdateHandler>(20);
            Bind(updateHandlerRegistry);
            
            var ownerPlayerRegistry = new OwnerPlayerRegistry();
            Bind(ownerPlayerRegistry);
                
            var spawnPointRegistry = new Registry<SpawnPoint>(20);
            Bind(spawnPointRegistry);
            
            var camera = GetComponentInChildren<Camera>(true);
            var cameraFollowHandler = GetComponentInChildren<CameraFollower>(true);
            cameraFollowHandler.Init(camera, ownerPlayerRegistry);

            var cameraBoundsUpdateHandler = GetComponentInChildren<CameraBoundsUpdateHandler>(true);
            cameraBoundsUpdateHandler.Init(camera);
            updateHandlerRegistry.Add(cameraBoundsUpdateHandler);
            
            var inputUpdateHandler = GetComponentInChildren<InputUpdateHandler>(true);
            inputUpdateHandler.Init();
            updateHandlerRegistry.Add(inputUpdateHandler);

            var ownerPlayerInputHandler = GetComponentInChildren<OwnerPlayerInputHandler>(true);
            ownerPlayerInputHandler.Init(inputUpdateHandler, camera, ownerPlayerRegistry);
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

            GetComponentInChildren<PresetMap>(true).Init(cameraBoundsUpdateHandler, spawnPointRegistry);
                            
            var tankFactory = GetComponentInChildren<TankFactory>(true);
            var player = GetComponentInChildren<LocalPlayer>();

            tankFactory.Init();
            player.Init(tankFactory.Create(spawnPointRegistry.Items[0].Position));
            ownerPlayerRegistry.Add(player);
            
            GetComponentInChildren<GameUpdater>(true).Init(updateHandlerRegistry);
        }
        
        private async void Start()
        {
            try
            {
                await GetComponentInChildren<UnityServicesInitializer>().Init();

                var activeSessionsController = GetComponentInChildren<ActiveSessionsController>(true);
                var activeSessionsView = GetComponentInChildren<ActiveSessionsView>(true);

                await activeSessionsController.Init();
                activeSessionsView.Init(activeSessionsController);
            }
            catch (Exception e)
            {
                Debug.LogError(e, this);
            }
        }
    }
}