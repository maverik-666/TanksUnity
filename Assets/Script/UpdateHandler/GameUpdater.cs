namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class GameUpdater : MonoBehaviour
    {
        private Registry<IUpdateHandler> _updateHandlerRegistry = null;
        private bool _isInitialized = false;

        public void Init(Registry<IUpdateHandler> updateHandlerRegistry)
        {
            _updateHandlerRegistry = updateHandlerRegistry;
            _isInitialized = true;
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            foreach (var updateHandler in _updateHandlerRegistry.Items)
            {
                if (updateHandler.Enabled)
                {
                    updateHandler.UpdateTick(deltaTime);
                }
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