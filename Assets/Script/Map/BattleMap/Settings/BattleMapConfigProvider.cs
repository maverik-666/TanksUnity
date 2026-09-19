namespace Project
{
    using System;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class BattleMapConfigProvider : MonoBehaviour
    {
        [SerializeField]
        private CounterSetting _widthSetting = null;
        
        [SerializeField]
        private CounterSetting _heightSetting = null;
        
        [SerializeField]
        private ContentCounterSetting[] _contentSettings = null;

        private PlayerRegistry _playerRegistry = null;
        private bool _isInitialized = false;

        public void Init(PlayerRegistry playerRegistry)
        {
            _widthSetting.Init();
            _heightSetting.Init();
            foreach (var contentSetting in _contentSettings)
            {
                contentSetting.Init();
            }
            
            _playerRegistry = playerRegistry;
            _isInitialized = true;
        }

        public BattleMapConfig GetBattleMapConfig()
        {
            var size = new Vector2Int(_widthSetting.Value, _heightSetting.Value);
            
            var contentChances = new ContentChance[_contentSettings.Length];
            for (int i = 0; i < contentChances.Length; i++)
            {
                contentChances[i] = new ContentChance()
                {
                    ContentId = _contentSettings[i].ContentId.ID,
                    Chance = _contentSettings[i].Value/100f
                };
            }
            
            return new BattleMapConfig()
            {
                Size = size,
                Content = contentChances,
                SpawnCount = _playerRegistry.Count
            };
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(BattleMapConfigProvider)} not initialized", this);
            }
        }
#endif

        [Serializable]
        private class ContentCounterSetting
        {
            [SerializeField]
            private MapContentId _contentId;

            [SerializeField]
            private CounterSetting _counterSetting;

            public MapContentId ContentId => _contentId;

            public int Value => _counterSetting.Value;

            public void Init()
            {
                _counterSetting.Init();
            }
        }
    }
}