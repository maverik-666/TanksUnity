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
        
        public void Awake()
        {
            var player = GetComponentInChildren<Player>(true);
            
            var camera = GetComponentInChildren<Camera>(true);
            
            _inputSystem = GetComponentInChildren<InputSystem>(true);
            _playerInputHandler = GetComponentInChildren<PlayerInputHandler>(true);
            _timerUpdateHandler = GetComponentInChildren<TimerUpdateHandler>(true);
            
            player.Init();
            
            _inputSystem.Init();
            _playerInputHandler.Init(_inputSystem, camera, player);
            _timerUpdateHandler.Init();
            
            GetComponentInChildren<TurretAimUseCase>(true).Init();
            GetComponentInChildren<HullMovementUseCase>(true).Init();
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
        }
    }
}