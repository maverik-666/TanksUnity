namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class BoxFactory : MonoBehaviour, IContentFactory
    {
        [SerializeField]
        private MapContentId _contentId = null;
        
        [SerializeField]
        private Destructible _boxPf = null;
        
        private bool _isInitialized = false;

        public MapContentId ContentId => _contentId;
        
        public void Init()
        {
            _isInitialized = true;
        }
        
        public void CreateOn(Vector2 pos)
        {
            
            var box = Instantiate(_boxPf, pos, Quaternion.Euler(0,0, 90 * Random.Range(0, 4)), transform);
            box.Init();
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(BoxFactory)} not initialized", this);
            }
        }
#endif
    }
}