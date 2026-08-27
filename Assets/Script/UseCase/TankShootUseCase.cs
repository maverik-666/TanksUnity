namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class TankShootUseCase : MonoBehaviour
    {
        [SerializeField]
        private float _recoil = 3f;
        
        private bool _isInitialized = false;
        
        public void Init()
        {
            _isInitialized = true;
        }

        public void Execute(Hull hull, Turret turret)
        {
            if(turret.CooldownTimer.IsRunning)
                return;
            
            hull.ApplyRecoil(turret.Rotation * Vector3.right * -_recoil);
            turret.CooldownTimer.Setup(turret.TurretStats.ShootCooldown);
            turret.AnimateShoot();
            SpawnProjectile();
        }

        private void SpawnProjectile()
        {
            //TODO:
        } 
        
#if UNITY_EDITOR  
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(TankShootUseCase)} not initialized", this);
            }
        }
#endif
    }
}