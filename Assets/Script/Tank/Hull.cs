namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Hull : MonoBehaviour
    {
        public HullStats HullStats => _hullStats;
        public Rigidbody2D Rigidbody2D => _rigidbody2D;
        
        private HullStats _hullStats;
        
        private Rigidbody2D _rigidbody2D = null;
        private HullMovementUseCase _hullMovementUseCase = null;
        
        private bool _isInitialized = false;
        
        public void Init(HullStats hullStats)
        {
            _hullStats = hullStats;
            _hullMovementUseCase = this.Resolve<HullMovementUseCase>();
            
            _rigidbody2D = GetComponentInChildren<Rigidbody2D>(true);
            
            _isInitialized = true;
        }
        
        public void Move(Vector2 input, float deltaTime)
        {
            _hullMovementUseCase.Execute(this, input, deltaTime);
        }
        
        public void ApplyRecoil(Vector2 force)
        {
            _rigidbody2D.AddForce(force, ForceMode2D.Impulse);
        }
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(Hull)} not initialized", this);
            }
        }
#endif
    }
}