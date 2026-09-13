namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class PresetMap : MonoBehaviour
    {
        private bool _isInitialized = false;

        public void Init(CameraBoundsUpdateHandler cameraBounds, Registry<SpawnPoint> spawnPointRegistry)
        {
            var border = GetComponentInChildren<EdgeCollider2D>();
            var bounds = border.bounds;
            bounds.size += Vector3.one;
            cameraBounds.SetBounds(bounds);
            
            spawnPointRegistry.AddRange(GetComponentsInChildren<SpawnPoint>(true));
            
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