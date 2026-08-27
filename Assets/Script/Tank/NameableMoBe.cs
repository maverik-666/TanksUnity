namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class NameableMoBe : MonoBehaviour
    {
    
        private bool _isInitialized = false;
        
        public void Init()
        {
            _isInitialized = true;
        }
        
#if UNITY_EDITOR  
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(NameableMoBe)} not initialized", this);
            }
        }
#endif
    }
}