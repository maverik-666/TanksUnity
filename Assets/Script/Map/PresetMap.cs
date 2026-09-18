namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class PresetMap : MonoBehaviour, IMap
    {
        private SpawnPoint[] _spawnPoints;
        private Bounds _bounds;
        private bool _isInitialized = false;
        
        public Bounds Bounds => _bounds;

        public void Init()
        {
            var border = GetComponentInChildren<EdgeCollider2D>();
            _bounds = border.bounds;
            _bounds.size += Vector3.one;

            _spawnPoints = GetComponentsInChildren<SpawnPoint>(true);
            
            foreach (var destructible in GetComponentsInChildren<Destructible>(true))
            {
                destructible.Init();
            }
            
            foreach (var bush in GetComponentsInChildren<Bush>(true))
            {
                bush.Init();
            }
            
            _isInitialized = true;
        }
        
        public SpawnPoint GetSpawnPoint()
        {
            return _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(PresetMap)} not initialized", this);
            }
        }
#endif
    }
}