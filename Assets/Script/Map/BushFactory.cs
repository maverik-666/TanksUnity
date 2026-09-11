namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class BushFactory : MonoBehaviour, IContentFactory
    {
        [SerializeField]
        private MapContentId _contentId = null;
        
        [SerializeField]
        private Bush _bushPf = null;
        
        private bool _isInitialized = false;

        public MapContentId ContentId => _contentId;

        public void Init()
        {
            _isInitialized = true;
        }
        
        public void CreateOn(Vector2 pos)
        {
            var bush = Instantiate(_bushPf, pos, Quaternion.Euler(0,0, 90 * Random.Range(0, 4)), transform);
            bush.Init();
        }
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(BushFactory)} not initialized", this);
            }
        }
#endif
    }
}