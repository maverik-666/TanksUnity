namespace Project
{
    using System;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class GameUpdater : MonoBehaviour
    {
        private IUpdateHandler[] _updateHandlers = null;
        private bool _isInitialized = false;

        public void Init(params IUpdateHandler[] updateHandlers)
        {
            _updateHandlers = updateHandlers;
            _isInitialized = true;
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            foreach (var updateHandler in _updateHandlers)
            {
                updateHandler.UpdateTick(deltaTime);
            }
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(GameUpdater)} not initialized", this);
            }
        }
#endif
    }
}