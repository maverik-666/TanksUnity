namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Tank))]
    public class Player : MonoBehaviour
    {
        public Tank Tank => _tank;
        
        private Tank _tank;
        private bool _isInitialized = false;
        
        public void Init()
        {
            _tank = GetComponent<Tank>();
            _isInitialized = true;
        }
        
#if UNITY_EDITOR  
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(Player)} not initialized", this);
            }
        }
#endif
    }
}