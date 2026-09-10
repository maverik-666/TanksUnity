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
        
        public void CreateOn(int x, int y)
        {
            var bush = Instantiate(_bushPf, new Vector3(x,y,0), Quaternion.identity, transform);
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