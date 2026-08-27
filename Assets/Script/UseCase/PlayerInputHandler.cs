namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class PlayerInputHandler : MonoBehaviour
    {
        private Camera _camera = null;
        private InputSystem _inputSystem = null;
        private Player _player = null;
        private bool _isInitialized = false;
        
        public void Init(InputSystem inputSystem, Camera camera, Player player)
        {
            _inputSystem = inputSystem;
            _camera = camera;
            _player = player;

            _inputSystem.ShootListener.Pressed += Shoot;
            
            _isInitialized = true;
        }

        public void UpdateTick(float deltaTime)
        {
            Move(deltaTime);
            Rotate(deltaTime);
        }

        private void Move(float deltaTime)
        {
            _player.Tank.Move(_inputSystem.MoveInput, deltaTime);

        }

        private void Rotate(float deltaTime)
        {
            var mousePosition = _camera.ScreenToWorldPoint(_inputSystem.MouseScreenPosition);
            
            _player.Tank.Rotate(mousePosition, deltaTime);
        }

        private void Shoot()
        {
            _player.Tank.Shoot();
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
                Debug.LogError($"{nameof(PlayerInputHandler)} not initialized", this);
            }
        }
#endif
    }
}