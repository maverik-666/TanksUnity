namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class Bush : MonoBehaviour
    {
        [SerializeField]
        private OneShotVFXId _vfxId;
        
        private OneShotVFXFactory _oneShotVFXFactory;
        private bool _isInitialized = false;

        public void Init()
        {
            _oneShotVFXFactory = this.Resolve<OneShotVFXFactory>();
            
            _isInitialized = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _oneShotVFXFactory.Create(_vfxId, other.transform.position);
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(Bush)} not initialized", this);
            }
        }
#endif
    }
}