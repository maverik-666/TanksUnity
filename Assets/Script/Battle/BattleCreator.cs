namespace Project
{
    using System;
    using System.Collections.Generic;
    using Unity.Netcode;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class BattleCreator : NetworkBehaviour
    {
        private CameraBoundsUpdateHandler _cameraBounds = null;
        private PresetMap _lobbyMap = null;
        private PlayerRegistry _playerRegistry = null;

        private BattleMapGenerator _battleMapGenerator = null;
        private BattleMapConfigProvider _battleMapConfigProvider = null;
        private PlayBattleButton _playBattleButton = null;
        private bool _isInitialized = false;

        private List<Health> _alivePLayers = new List<Health>();
        
        public void Init(
            CameraBoundsUpdateHandler cameraBounds,
            PlayerRegistry playerRegistry,
            PresetMap lobbyMap,
            InteractableRegistrator interactableRegistrator,
            BattleMapGenerator battleMapGenerator,
            BattleMapConfigProvider battleMapConfigProvider)
        {
            _cameraBounds = cameraBounds;
            _lobbyMap = lobbyMap;
            _playerRegistry = playerRegistry;
            
            _battleMapGenerator = battleMapGenerator;
            _battleMapConfigProvider = battleMapConfigProvider;

            _playBattleButton = GetComponentInChildren<PlayBattleButton>();
            _playBattleButton.Init(this);
            interactableRegistrator.Register(_playBattleButton);
            
            _cameraBounds.SetBounds(_lobbyMap.Bounds);
            
            _isInitialized = true;
        }

        public void StartBattle()
        {
            var config = _battleMapConfigProvider.GetBattleMapConfig();
            _battleMapGenerator.Generate(config, 0);
            _cameraBounds.SetBounds(_battleMapGenerator.Bounds);

            StartBattleRpc(config);

            for (int i = 0; i < _playerRegistry.Count; i++)
            {
                var spawnPoint = _battleMapGenerator.SpawnPoints[i % _battleMapGenerator.SpawnPoints.Count];
                _playerRegistry.Items[i].Teleport(spawnPoint.Position);
                _playerRegistry.Items[i].SetInvincibility(false);
                _alivePLayers.Add(_playerRegistry.Items[i].Health);
            }
        }

        [Rpc(SendTo.NotServer)]
        private void StartBattleRpc(BattleMapConfig config)
        {
            _battleMapGenerator.Generate(config, 0);
            _cameraBounds.SetBounds(_battleMapGenerator.Bounds);
            
            for (int i = 0; i < _playerRegistry.Count; i++)
            {
                _playerRegistry.Items[i].Revive();
                _playerRegistry.Items[i].SetInvincibility(false);
            }
        }

        public void OnDamageTakenBy(IDamageable damageable)
        {
            if (_alivePLayers.Contains(damageable.Health))
            {
                if (damageable.IsDead)
                {
                    OnPlayerDeath(damageable.Health);
                }
            }
        }

        private void OnPlayerDeath(Health health)
        {
            _alivePLayers.Remove(health);

            if (_alivePLayers.Count <= 1)
            {
                EndBattle();
            }
        }

        private void EndBattle()
        {
            _cameraBounds.SetBounds(_lobbyMap.Bounds);

            EndBattleRpc();

            for (int i = 0; i < _playerRegistry.Count; i++)
            {
                var spawnPoint = _lobbyMap.SpawnPoints[i % _lobbyMap.SpawnPoints.Count];
                _playerRegistry.Items[i].Teleport(spawnPoint.Position);
                _playerRegistry.Items[i].SetInvincibility(true);
                _playerRegistry.Items[i].Revive();
                
            }
        }
        
        [Rpc(SendTo.NotServer)]
        private void EndBattleRpc()
        {
            _cameraBounds.SetBounds(_lobbyMap.Bounds);
            
            for (int i = 0; i < _playerRegistry.Count; i++)
            {
                _playerRegistry.Items[i].SetInvincibility(true);
                _playerRegistry.Items[i].Revive();
            }
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(BattleCreator)} not initialized", this);
            }
        }
#endif
    }
}