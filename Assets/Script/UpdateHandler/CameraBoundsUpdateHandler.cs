namespace Project
{
    using System;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class CameraBoundsUpdateHandler : MonoBehaviour, IUpdateHandler
    {
        private Camera _camera = null;
        private Bounds _bounds;
        private bool _hasBounds = false;
        private bool _isInitialized = false;
        
        public bool Enabled => enabled;
        
        public void Init(Camera camera)
        {
            _camera = camera;
            _isInitialized = true;
        }

        public void SetBounds(Bounds bounds)
        {
            _bounds = bounds;
            _hasBounds = true;
        }
        
        public void UpdateTick(float deltaTime)
        {
            if (!_hasBounds)
            {
                return;
            }
            
            float camHalfHeight = _camera.orthographicSize;
            float camHalfWidth = camHalfHeight * _camera.aspect;

            var pos = _camera.transform.position;
                
            float limitMinX = _bounds.min.x + camHalfWidth;
            float limitMaxX = _bounds.max.x - camHalfWidth;
            float limitMinY = _bounds.min.y + camHalfHeight;
            float limitMaxY = _bounds.max.y - camHalfHeight;

            pos.x = limitMinX > limitMaxX ? (_bounds.min.x + _bounds.max.x) / 2 : Mathf.Clamp(pos.x, limitMinX, limitMaxX);
            pos.y = limitMinY > limitMaxY ? (_bounds.min.y + _bounds.max.y) / 2 : Mathf.Clamp(pos.y, limitMinY, limitMaxY);

            _camera.transform.position = pos;
        }
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(CameraBoundsUpdateHandler)} not initialized", this);
            }
        }
#endif
    }
}