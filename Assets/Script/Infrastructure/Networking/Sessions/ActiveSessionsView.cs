namespace Project
{
    using System;
    using System.Collections.Generic;
    using Unity.Services.Multiplayer;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class ActiveSessionsView : MonoBehaviour
    {
        [SerializeField]
        private Transform _sessionButtonSpawnPoint = null;
        
        [SerializeField]
        private SessionButton _sessionButtonPf = null;
        
        private ActiveSessionsController _activeSessionsController = null;
        private Dictionary<string, SessionButton> _sessionButtons = null;
        
        private bool _isInitialized = false;

        public void Init(ActiveSessionsController activeSessionsController)
        {
            _sessionButtons = new Dictionary<string, SessionButton>();

            _activeSessionsController = activeSessionsController;
            _activeSessionsController.SessionAdded += CreateSessionButtonFor;
            _activeSessionsController.SessionStateUpdated += UpdateSessionButtonOf;
            _activeSessionsController.SessionRemoved += RemoveSessionButtonFor;

            ShowActiveSessions();
            
            _isInitialized = true;
        }

        private void ShowActiveSessions()
        {
            foreach (var session in _activeSessionsController.ActiveSessions)
            {
                CreateSessionButtonFor(session);
            }
        }
        
        private void CreateSessionButtonFor(ISession session)
        {
            var sessionButton = Instantiate(_sessionButtonPf, _sessionButtonSpawnPoint.position, Quaternion.identity, transform);
            sessionButton.Init(TryJoin, session.Id, session.Name, _activeSessionsController.GetGameStateOf(session.Id));
            
            _sessionButtons[session.Id] = sessionButton;
        }
        
        private void TryJoin(string sessionId)
        {
            Debug.Log($"Try Joining {sessionId}");
        }
        
        private void UpdateSessionButtonOf(ISession session, GameState gameState)
        {
            _sessionButtons[session.Id].UpdateState(gameState);
        }
        
        private void RemoveSessionButtonFor(ISession session)
        {
            if (_sessionButtons.TryGetValue(session.Id, out var button))
            {
                Destroy(button);
            }
        }
        
        private void OnDestroy()
        {
            if (_activeSessionsController != null)
            {
                _activeSessionsController.SessionAdded -= CreateSessionButtonFor;
                _activeSessionsController.SessionStateUpdated -= UpdateSessionButtonOf;
                _activeSessionsController.SessionRemoved -= RemoveSessionButtonFor;
                
            }
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(ActiveSessionsView)} not initialized", this);
            }
        }
#endif
    }
}