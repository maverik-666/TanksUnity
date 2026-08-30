namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class Map : MonoBehaviour
    {
        public Vector2 Size => _size;

        private Vector2 _size;
        private bool _isInitialized = false;

        public void Init(Vector2 size)
        {
            _size = size;
            _isInitialized = true;
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(Map)} not initialized", this);
            }
        }
#endif
    }
}