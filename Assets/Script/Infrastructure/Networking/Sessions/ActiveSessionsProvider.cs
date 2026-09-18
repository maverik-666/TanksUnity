namespace Project
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Unity.Services.Multiplayer;
    using UnityEngine;
    
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class ActiveSessionsProvider : MonoBehaviour
    {
        private IMultiplayerService _multiplayerService;
        
        private List<string> _activeSessions;
        private Dictionary<string, GameState> _activeSessionStates;

        private QuerySessionsOptions _querySessionsOptions;

        public Action<ISessionInfo, GameState> SessionAdded;
        public Action<string> SessionRemoved;
        public Action<string, GameState> SessionStateUpdated;
        
        public void Init()
        {
            _multiplayerService = MultiplayerService.Instance;
            
            _querySessionsOptions = new QuerySessionsOptions()
            {
                Count = 20,
            };
            
            _activeSessions = new List<string>();
            _activeSessionStates =  new Dictionary<string, GameState>();
            
            InvokeRepeating(nameof(PollSessions), 0, 5);
        }

        private async void PollSessions()
        {
            try
            {
                await RefreshActiveSessions();
            } 
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private async Task RefreshActiveSessions()
        {
            var querySessionsResults = await _multiplayerService.QuerySessionsAsync(_querySessionsOptions);

            foreach (var sessionInfo in querySessionsResults.Sessions)
            {
                if (_activeSessions.Contains(sessionInfo.Id))
                {
                    RefreshSession(sessionInfo);
                }
                else
                {
                    OnSessionAdded(sessionInfo);
                }
            }
            
            foreach (string sessionId in _activeSessions)
            {
                bool isRemove = true;
                foreach (var sessionInfo in querySessionsResults.Sessions)
                {
                    if (sessionId == sessionInfo.Id)
                    {
                        isRemove = false;
                        break;
                    }
                }

                if (isRemove)
                {
                    OnSessionRemoved(sessionId);
                }
            }
        }

        private void OnSessionAdded(ISessionInfo sessionInfo)
        {
            _activeSessions.Add(sessionInfo.Id);

            var gameState = CalcGameStateOf(sessionInfo);
            
            _activeSessionStates.Add(sessionInfo.Id, gameState);
            
            SessionAdded?.Invoke(sessionInfo, gameState);
        }

        private void RefreshSession(ISessionInfo sessionInfo)
        { 
            var gameState = CalcGameStateOf(sessionInfo);
            if (_activeSessionStates[sessionInfo.Id] == gameState)
            {
                return;
            }
            
            _activeSessionStates[sessionInfo.Id] = gameState;
            SessionStateUpdated?.Invoke(sessionInfo.Id, gameState);
        }
        
        private void OnSessionRemoved(string sessionId)
        {
            _activeSessions.Remove(sessionId);
            _activeSessionStates.Remove(sessionId);
            
            SessionRemoved?.Invoke(sessionId);
        }

        private GameState CalcGameStateOf(ISessionInfo sessionInfo)
        {
            if (!sessionInfo.Properties.TryGetValue("gameState", out var property))
            {
                return GameState.Invalid;
            }
            
            switch (property.Value)
            {
                case "Wait":
                    return GameState.Wait;
                case "InGame":
                    return GameState.InGame;
            }

            return GameState.Invalid;
        }

        private void OnDestroy()
        {
            CancelInvoke(nameof(PollSessions));
        }
    }

    public enum GameState
    {
        Invalid,
        Wait,
        InGame,
    }
}