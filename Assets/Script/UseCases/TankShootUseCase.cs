namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class TankShootUseCase : MonoBehaviour
    {
        [SerializeField]
        private float _recoil = 3f;

        private IProjectileFactory _projectileFactory = null;
        private bool _isInitialized = false;
        
        public void Init(IProjectileFactory projectileFactory)
        {
            _projectileFactory = projectileFactory;
            _isInitialized = true;
        }

        public void Execute(Tank tank, bool isInstant)
        {
            var turret = tank.Turret;
            
            if(turret.CooldownTimer.IsRunning && !isInstant)
                return;
            
            tank.ApplyRecoil(turret.Rotation * Vector3.right * -_recoil);
            turret.CooldownTimer.Setup(turret.TurretStats.ShootCooldown);
            turret.AnimateShoot();
            ShootProjectileBy(tank);
        }

        private void ShootProjectileBy(Tank tank)
        { 
            _projectileFactory.Create(tank, tank.Turret.ProjectilePosition, tank.Turret.TurretStats.Damage, tank.Turret.Rotation);
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