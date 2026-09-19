namespace Project
{
    using Unity.Netcode;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class BattleCreator : NetworkBehaviour
    {
        private CameraBoundsUpdateHandler _cameraBounds = null;
        private PresetMap _lobbyMap = null;
        private PlayerRegistry _playerRegistry = null;

        private DestructibleRegistrator _destructibleRegistrator = null;
        private BattleMapGenerator _battleMapGenerator = null;
        private BattleMapConfigProvider _battleMapConfigProvider = null;
        private PlayBattleButton _playBattleButton = null;
        private bool _isInitialized = false;

        public void Init(CameraBoundsUpdateHandler cameraBounds, PlayerRegistry playerRegistry, PresetMap lobbyMap, DestructibleRegistrator destructibleRegistrator, BattleMapGenerator battleMapGenerator, BattleMapConfigProvider battleMapConfigProvider)
        {
            _cameraBounds = cameraBounds;
            _lobbyMap = lobbyMap;
            _playerRegistry = playerRegistry;
            
            _destructibleRegistrator = destructibleRegistrator;
            _battleMapGenerator = battleMapGenerator;
            _battleMapConfigProvider = battleMapConfigProvider;

            _playBattleButton = GetComponentInChildren<PlayBattleButton>();
            _playBattleButton.Init(this);

            _cameraBounds.SetBounds(_lobbyMap.Bounds);
            
            _isInitialized = true;
        }

        public void StartBattle()
        {
            _destructibleRegistrator.ResetCounter();
            
            var config = _battleMapConfigProvider.GetBattleMapConfig();
            _battleMapGenerator.Generate(config, 0);
            _cameraBounds.SetBounds(_battleMapGenerator.Bounds);

            StartBattleRpc(config);
            
            for (int i = 0; i < _battleMapGenerator.SpawnPoints.Count; i++)
            {
                var spawnPoint = _battleMapGenerator.SpawnPoints[i];
                _playerRegistry.Items[i].Teleport(spawnPoint.Position);
            }
        }

        [Rpc(SendTo.NotServer)]
        private void StartBattleRpc(BattleMapConfig config)
        {
            _destructibleRegistrator.ResetCounter();
            _battleMapGenerator.Generate(config, 0);
            _cameraBounds.SetBounds(_battleMapGenerator.Bounds);
        }

        [ContextMenu("Aboba")]
        public void EndBattle()
        {
            _cameraBounds.SetBounds(_lobbyMap.Bounds);
            
            EndBattleRpc();
            
            for (int i = 0; i < _lobbyMap.SpawnPoints.Count; i++)
            {
                var spawnPoint = _lobbyMap.SpawnPoints[i];
                _playerRegistry.Items[i].Teleport(spawnPoint.Position);
            }
        }
        
        [Rpc(SendTo.NotServer)]
        private void EndBattleRpc()
        {
            _cameraBounds.SetBounds(_lobbyMap.Bounds);
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