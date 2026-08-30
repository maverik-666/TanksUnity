namespace Project
{
    using System;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class OneShotVFXFactory : MonoBehaviour
    {
        [SerializeField]
        private OneShotVFXPrefabById[] _vfxPfs = null;
        
        private bool _isInitialized = false;

        public void Init()
        { 
            _isInitialized = true;
        }

        public void Create(OneShotVFXId id, Vector3 position)
        {
            if (!TryGetPrefabBy(id, out var prefab))
            {
                Debug.LogError($"prefab with id {id} not found", this);
                return;
            }
                
            var vfx = Instantiate(prefab, position, Quaternion.identity, transform);
            vfx.Init();
            
            vfx.Play(() => Destroy(vfx.gameObject));
        }

        private bool TryGetPrefabBy(OneShotVFXId id, out OneShotVFX prefab)
        {
            foreach (var vfxPf in _vfxPfs)
            {
                if (vfxPf.Id == id)
                {
                    prefab = vfxPf.Prefab;
                    return true;
                }
            }

            prefab = null;
            return false;
        }
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(OneShotVFXFactory)} not initialized", this);
            }
        }
#endif
    }

    [Serializable]
    public class OneShotVFXPrefabById
    {
        [SerializeField] 
        private OneShotVFXId _id = null;
        
        [SerializeField] 
        private OneShotVFX _prefab = null;

        public OneShotVFXId Id => _id;
        public OneShotVFX Prefab => _prefab;
    }
}