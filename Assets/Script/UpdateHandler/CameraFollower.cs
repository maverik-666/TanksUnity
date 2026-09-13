namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class CameraFollower : MonoBehaviour
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
        
        public void FixedUpdate()
        {
            if (_playerRegistry.Count == 0)
            {
                return;
            }

            var tankPos = _playerRegistry.Items[0].TankPosition;
            _pos = Vector2.Lerp(_pos, tankPos, 0.1f);
            
            if (Mathf.Abs(_pos.x - tankPos.x) < 0.01f 
                && Mathf.Abs(_pos.y - tankPos.y) < 0.01f)
            {
                _pos = tankPos;
            }
                
            _camera.transform.position = new Vector3(_pos.x, _pos.y, -10);
        }

        public void Teleport()
        {
            _pos = _playerRegistry.Items[0].TankPosition;
        } 
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(CameraFollower)} not initialized", this);
            }
        }
#endif
    }
}