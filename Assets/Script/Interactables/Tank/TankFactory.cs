namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class TankFactory : MonoBehaviour
    {
        [SerializeField]
        private TankStatsSO _tankStats;
        
        [SerializeField]
        private Tank _tankPf;
        
        private bool _isInitialized = false;

        public void Init()
        {
            _isInitialized = true;
        }

        public Tank Create(IPlayer player, Vector3 position)
        {
            var tank  = Instantiate(_tankPf, position, Quaternion.identity, transform);
            tank.Init(player, _tankStats.TankStats);

            return tank;
        }
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(TankFactory)} not initialized", this);
            }
        }
#endif
    }
}