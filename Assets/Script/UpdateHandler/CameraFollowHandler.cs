namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class CameraFollowHandler : MonoBehaviour, IUpdateHandler
    {
        private OwnerPlayerRegistry _playerRegistry;
        private Camera _camera = null;
        private bool _isInitialized = false;

        private Vector2 _pos;

        public void Init(Camera cam, OwnerPlayerRegistry playerRegistry)
        {
            _camera = cam;
            _playerRegistry = playerRegistry;
            _isInitialized = true;
        }
        
        public void UpdateTick(float deltaTime)
        {
            if (_playerRegistry.Count == 0)
            {
                return;
            }
            
            _pos = Vector2.Lerp(_pos, _playerRegistry.Items[0].transform.position, 0.1f);
            _camera.transform.position = new Vector3(_pos.x, _pos.y, -10);
        }

        public void Teleport()
        {
            _pos = _playerRegistry.Items[0].transform.position;
        } 
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(CameraFollowHandler)} not initialized", this);
            }
        }
#endif
    }
}