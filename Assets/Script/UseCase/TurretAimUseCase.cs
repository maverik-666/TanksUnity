namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class TurretAimUseCase : MonoBehaviour
    {
        private bool _isInitialized = false;
        
        public void Init()
        {
            _isInitialized = true;
        }

        public void Execute(Turret turret, Vector2 aimTarget, float deltaTime)
        {
            var direction = aimTarget - turret.Position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    
            var aimRotation = Quaternion.Euler(0, 0, angle);

            turret.transform.rotation = Quaternion.RotateTowards(turret.Rotation, aimRotation, turret.TurretStats.RotationSpeed * deltaTime);
        }
        
#if UNITY_EDITOR  
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(TurretAimUseCase)} not initialized", this);
            }
        }
#endif
    }
}