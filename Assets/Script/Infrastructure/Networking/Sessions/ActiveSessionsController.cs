namespace Project
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Unity.Services.Multiplayer;
    using UnityEngine;
    
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class ActiveSessionsController : MonoBehaviour
    {
        private IMultiplayerService _multiplayerService;
        
        private List<ISession> _activeSessions;
        private SessionOptions _sessionOptions;
        private QuerySessionsOptions _querySessionsOptions;
        private bool _isInitialized = false;

        public IReadOnlyList<ISession> ActiveSessions => _activeSessions;
        
        public Action<ISession> SessionAdded;
        public Action<ISession> SessionRemoved;
        public Action<ISession, GameState> SessionStateUpdated;
        
        public async Task Init()
        {
            _multiplayerService = MultiplayerService.Instance;
            
            _sessionOptions = new SessionOptions()
            {
                Name = "Aboba",
                MaxPlayers = 4,
            }.WithRelayNetwork();
            
            _querySessionsOptions = new QuerySessionsOptions()
            {
                Count = 20,
            };

            _multiplayerService.SessionAdded += OnSessionAdded;
            _multiplayerService.SessionRemoved += OnSessionRemoved;
            
            _activeSessions = new List<ISession>();
            
            var querySessionsResults = await _multiplayerService.QuerySessionsAsync(_querySessionsOptions);
            foreach (var sessionInfo in querySessionsResults.Sessions)
            {
                var session = GetSessionById(sessionInfo.Id);
                _activeSessions.Add(session);
                
                OnSessionAdded(session);
            }
            
            _isInitialized = true;
        }

        private void OnSessionAdded(ISession session)
        {
            session.SessionPropertiesChanged += OnPropertyChange;
                
            void OnPropertyChange()
            {
                SessionStateUpdated?.Invoke(session, GetGameStateOf(session.Id));
            }
            
            SessionAdded?.Invoke(session);
        }
        
        private void OnSessionRemoved(ISession session)
        {
            _activeSessions.Remove(session);
            
            SessionRemoved?.Invoke(session);
        }
        
        public async void HostSession()
        {
            var session = await _multiplayerService.CreateSessionAsync(_sessionOptions);
            
            Debug.Log($"Created session: {session.Id}", this);
        }
        
        public GameState GetGameStateOf(string sessionId)
        {
            GetSessionById(sessionId).Properties.TryGetValue("gameState", out var property);

            
            switch (property.Value)
            {
                case "Wait":
                    return GameState.Wait;
                case "InGame":
                    return GameState.InGame;
            }

            return GameState.Invalid;
        }

        public async void JoinSession(string sessionId)
        {
            await _multiplayerService.JoinSessionByIdAsync(sessionId);
        }

        private ISession GetSessionById(string sessionId)
        {
            return _multiplayerService.Sessions[sessionId];
        }

        private void OnDestroy()
        {
            _multiplayerService.SessionAdded -= OnSessionAdded;
            _multiplayerService.SessionRemoved -= OnSessionRemoved;
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(ActiveSessionsController)} not initialized", this);
            }
        }
#endif
    }

    public enum GameState
    {
        Invalid,
        Wait,
        InGame,
    }
}