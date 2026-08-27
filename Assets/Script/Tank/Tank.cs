namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class Tank : MonoBehaviour
    {
        private TankStats _tankStats;
        
        private Turret _turret = null;
        private Hull _hull = null;

        private TankShootUseCase _tankShootUseCase = null;
        
        private bool _isInitialized = false;
        
        public void Init(TankStats tankStats)
        {
            _tankStats = tankStats;
            
            _turret = GetComponentInChildren<Turret>(true);
            _hull = GetComponentInChildren<Hull>(true);

            _tankShootUseCase = this.Resolve<TankShootUseCase>();
            
            _turret.Init(_tankStats.TurretStats);
            _hull.Init(_tankStats.HullStats);
            
            _isInitialized = true;
        }
        
        public void Move(Vector2 input, float deltaTime)
        {
            _hull.Move(input, deltaTime);
        }
        
        public void Rotate(Vector2 aimTarget, float deltaTime)
        {
            _turret.Rotate(aimTarget, deltaTime);
        }
        
        public void Shoot()
        {
            _tankShootUseCase.Execute(_hull, _turret);
        }
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(Tank)} not initialized", this);
            }
        }
#endif
    }
}