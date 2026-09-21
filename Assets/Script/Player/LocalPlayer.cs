namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class LocalPlayer : MonoBehaviour, IPlayer
    {
        private Tank _tank;
        private bool _isInitialized = false;

        public Vector2 TankPosition => _tank.transform.position;
        
        public PlayerPermissions Permissions => PlayerPermissions.All;
        
        public Health Health => _tank.Health;
        
        public void Init(Tank tank)
        {
            _tank = tank;
            
            _isInitialized = true;
        }
        
        public void SetInvincibility(bool isInvincible)
        {
            _tank.SetInvincibility(isInvincible);
        }
        
        public void Shoot()
        {
            _tank.Shoot(false);
        }

        public void Move(Vector2 input, float deltaTime)
        {
            _tank.Move(input, deltaTime);
        }

        public void Rotate(Vector2 targetPos, float deltaTime)
        {
            _tank.Rotate(targetPos, deltaTime);
        }

        public void Teleport(Vector2 targetPosition)
        {
            _tank.gameObject.SetActive(false);
            _tank.transform.position = targetPosition;
            _tank.gameObject.SetActive(true);
        }
        
        public void Revive()
        {
            _tank.Revive();
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(LocalPlayer)} not initialized", this);
            }
        }
#endif
  
    }
}