namespace Project
{
    using System.Collections.Generic;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class BoxFactory : MonoBehaviour, IContentFactory
    {
        [SerializeField]
        private MapContentId _contentId = null;
        
        [SerializeField]
        private Destructible _boxPf = null;

        private List<Destructible> _boxes = null;
        private DestructibleRegistrator _destructibleRegistrator = null;
        private bool _isInitialized = false;
        
        public MapContentId ContentId => _contentId;
        
        public void Init(DestructibleRegistrator destructibleRegistrator)
        {
            _boxes = new List<Destructible>();
            _destructibleRegistrator = destructibleRegistrator;
            _isInitialized = true;
        }
        
        public void CreateOn(Vector2 pos)
        {
            var box = Instantiate(_boxPf, pos, Quaternion.Euler(0,0, 90 * Random.Range(0, 4)), transform);
            box.Init();
            
            _boxes.Add(box);
            _destructibleRegistrator.Register(box);
        }

        public void ClearAll()
        {
            foreach (var box in _boxes)
            {
                if (_destructibleRegistrator.HasDestructible(box))
                {
                    _destructibleRegistrator.Unregister(box);
                    Destroy(box.gameObject);
                }
            }
            
            _boxes.Clear();
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