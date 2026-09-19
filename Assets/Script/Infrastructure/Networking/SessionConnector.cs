namespace Project
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Unity.Services.Authentication;
    using Unity.Services.Core;
    using Unity.Services.Multiplayer;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class SessionConnector : MonoBehaviour
    {
        private static SessionConnector _instance;

        private IMultiplayerService _multiplayerService;
        private SessionOptions _sessionOptions;
        
        private RequestType _pendingRequest = RequestType.None;
        private string _clientSessionId = string.Empty; 
        
        public static SessionConnector Instance => _instance;
        
        public void Init()
        {
            _instance = this;
            
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            
            _multiplayerService = MultiplayerService.Instance;
            
            _sessionOptions = new SessionOptions()
            {
                Name = "Aboba",
                MaxPlayers = 4,
                SessionProperties = new Dictionary<string, SessionProperty>()
                {
                    { "gameState", new SessionProperty("Wait") }
                }
            }.WithRelayNetwork();
        }

        public async void RequestHost()
        {
            try
            {
                _pendingRequest = RequestType.Host;
            
                await SceneManager.LoadSceneAsync("NetworkScene", LoadSceneMode.Single);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public async void RequestClient(string sessionId)
        {
            try
            {
                _pendingRequest = RequestType.Client;
                _clientSessionId = sessionId;
            
                await SceneManager.LoadSceneAsync("NetworkScene", LoadSceneMode.Single);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public async Task FulfillRequest()
        {
            if (_pendingRequest == RequestType.Host)
            {
                await _multiplayerService.CreateSessionAsync(_sessionOptions);
            }
            else if (_pendingRequest == RequestType.Client)
            {
                await _multiplayerService.JoinSessionByIdAsync(_clientSessionId);
            }
            
            _pendingRequest = RequestType.None;
        }
        
        public static async void HostInstant()
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            
            Debug.Log($"Sighed in! MY id is: {AuthenticationService.Instance.PlayerId}");
            
            await MultiplayerService.Instance.CreateSessionAsync(new SessionOptions()
            {
                Name = "Aboba",
                MaxPlayers = 4,
                SessionProperties = new Dictionary<string, SessionProperty>()
                {
                    { "gameState", new SessionProperty("Wait") }
                }
            }.WithRelayNetwork());
            
            Debug.Log($"Hosting!");
        }
        
        private enum RequestType
        {
            None, Host, Client
        }
    }
}