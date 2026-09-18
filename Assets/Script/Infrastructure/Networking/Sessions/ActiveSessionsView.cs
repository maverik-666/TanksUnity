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
        private ClientSessionButton _sessionButtonPf = null;

        private SessionConnector _sessionConnector = null;
        private ActiveSessionsProvider _activeSessionsProvider = null;
        private Dictionary<string, ClientSessionButton> _sessionButtons = null;
        

        public void Init(ActiveSessionsProvider activeSessionsProvider, SessionConnector sessionConnector)
        {
            _sessionButtons = new Dictionary<string, ClientSessionButton>();

            _sessionConnector = sessionConnector;
            _activeSessionsProvider = activeSessionsProvider;
            _activeSessionsProvider.SessionAdded += CreateSessionButtonFor;
            _activeSessionsProvider.SessionStateUpdated += UpdateSessionButtonOf;
            _activeSessionsProvider.SessionRemoved += RemoveSessionButtonFor;
        }
        
        private void CreateSessionButtonFor(ISessionInfo session, GameState gameState)
        {
            var sessionButton = Instantiate(_sessionButtonPf, _sessionButtonSpawnPoint.position, Quaternion.identity, transform);
            sessionButton.Init(TryJoin, session.Id, session.Name, gameState);
            
            _sessionButtons[session.Id] = sessionButton;
        }
        
        private void TryJoin(string sessionId)
        {
            _sessionConnector.RequestClient(sessionId);
        }
        
        private void UpdateSessionButtonOf(string sessionId, GameState gameState)
        {
            _sessionButtons[sessionId].UpdateState(gameState);
        }
        
        private void RemoveSessionButtonFor(string sessionId)
        {
            if (_sessionButtons.TryGetValue(sessionId, out var button))
            {
                Destroy(button);
            }
        }
        
        private void OnDestroy()
        {
            if (_activeSessionsProvider != null)
            {
                _activeSessionsProvider.SessionAdded -= CreateSessionButtonFor;
                _activeSessionsProvider.SessionStateUpdated -= UpdateSessionButtonOf;
                _activeSessionsProvider.SessionRemoved -= RemoveSessionButtonFor;
                
            }
        }
    }
}