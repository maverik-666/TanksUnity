namespace Project
{
    using UnityEngine;

    [RequireComponent(typeof(Transform))]
    public class OneShotVFXDestructibleStrategy : DestructibleStrategy
    {
        [SerializeField]
        private OneShotVFXId _oneShotVFXId = null;
        
        private OneShotVFXFactory _oneShotVFXFactory = null;
        private IDamageable _damageable = null;
        
        private bool _isInitialized = false;

        public override void Init(IDamageable damageable)
        {
            _damageable = damageable;
            
            _oneShotVFXFactory = this.Resolve<OneShotVFXFactory>();
            
            _isInitialized = true;
        }

        public override void Destruct()
        { 
            _oneShotVFXFactory.Create(_oneShotVFXId, _damageable.Position);
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(OneShotVFXDestructibleStrategy)} not initialized", this);
            }
        }
#endif
    }
}