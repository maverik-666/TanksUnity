namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class OwnerPlayerInputHandler : MonoBehaviour, IUpdateHandler
    {
        private Camera _camera = null;
        private InputSystem _inputSystem = null;
        private Registry<Player> _playerRegistry = null;
        private bool _isInitialized = false;
        
        public void Init(InputSystem inputSystem, Camera camera, Registry<Player> playerRegistry)
        {
            _inputSystem = inputSystem;
            _camera = camera;
            _playerRegistry = playerRegistry;
            _inputSystem.ShootListener.Pressed += Shoot;
            
            _isInitialized = true;
        }

        public void UpdateTick(float deltaTime)
        {
            if (_playerRegistry.Count == 0)
            {
                return;
            }
            
            Move(deltaTime);
            Rotate(deltaTime);
        }

        private void Move(float deltaTime)
        {
            foreach (var player in _playerRegistry.Items)
            { 
                player.Move(_inputSystem.MoveInput, deltaTime);
            }
        }

        private void Rotate(float deltaTime)
        {
            var mousePosition = _camera.ScreenToWorldPoint(_inputSystem.MouseScreenPosition);
            
            foreach (var player in _playerRegistry.Items)
            { 
                player.Rotate(mousePosition, deltaTime);
            }
        }

        private void Shoot()
        {
            foreach (var player in _playerRegistry.Items)
            { 
                player.Shoot();
            }
        }

        private void OnDestroy()
        {
            _inputSystem.ShootListener.Pressed -= Shoot;
        }

#if UNITY_EDITOR  
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(OwnerPlayerInputHandler)} not initialized", this);
            }
        }
#endif
    }
}