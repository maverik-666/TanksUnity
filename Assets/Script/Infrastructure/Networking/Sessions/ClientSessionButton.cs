namespace Project
{
    using System;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class ClientSessionButton : MonoBehaviour, IInteractable
    {
        private string _sessionId = string.Empty;
        private Action<string> _callback = null;
        private SessionButtonSkinRenderer _skinRenderer = null;
        
        private bool _isInitialized = false;

        public void Init(Action<string> callback, string sessionId, string sessionName, GameState gameState)
        {
            _sessionId = sessionId;
            _callback = callback;
            
            _skinRenderer = GetComponentInChildren<SessionButtonSkinRenderer>(true);
            _skinRenderer.Init(sessionName, gameState);
            
            _isInitialized = true;
        }

        public void UpdateState(GameState gameState)
        {
            _skinRenderer.UpdateState(gameState);
        }
        
        public void Interact(Projectile projectile)
        {
            _callback?.Invoke(_sessionId);
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(ClientSessionButton)} not initialized", this);
            }
        }
#endif
       
    }
}