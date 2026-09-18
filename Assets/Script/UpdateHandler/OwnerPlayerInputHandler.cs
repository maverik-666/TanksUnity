namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class OwnerPlayerInputHandler : MonoBehaviour, IUpdateHandler
    {
        private Camera _camera = null;
        private InputUpdateHandler _inputUpdateHandler = null;
        private OwnerPlayerRegistry _playerRegistry = null;
        private bool _isInitialized = false;

        public bool Enabled => enabled;
        
        public void Init(InputUpdateHandler inputUpdateHandler, Camera camera, OwnerPlayerRegistry playerRegistry)
        {
            _inputUpdateHandler = inputUpdateHandler;
            _camera = camera;
            _playerRegistry = playerRegistry;
            _inputUpdateHandler.ShootListener.Pressed += Shoot;
            
            _isInitialized = true;
        }
        
        public void UpdateTick(float deltaTime)
        {
            if (_playerRegistry.HasValue)
            {
                Move(deltaTime);
                Rotate(deltaTime);
            }
        }

        private void Move(float deltaTime)
        {
            _playerRegistry.Value.Move(_inputUpdateHandler.MoveInput, deltaTime);
        }

        private void Rotate(float deltaTime)
        {
            var mousePosition = _camera.ScreenToWorldPoint(_inputUpdateHandler.MouseScreenPosition);
            
            _playerRegistry.Value.Rotate(mousePosition, deltaTime);
        }

        private void Shoot()
        {
            _playerRegistry.Value.Shoot();
        }

        private void OnDestroy()
        {
            _inputUpdateHandler.ShootListener.Pressed -= Shoot;
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