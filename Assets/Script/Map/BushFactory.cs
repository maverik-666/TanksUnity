namespace Project
{
    using System.Collections.Generic;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class BushFactory : MonoBehaviour, IContentFactory
    {
        [SerializeField]
        private MapContentId _contentId = null;
        
        [SerializeField]
        private Bush _bushPf = null;

        private List<Bush> _bushes = null;
        private bool _isInitialized = false;

        public MapContentId ContentId => _contentId;

        public void Init()
        {
            _bushes = new List<Bush>();
            _isInitialized = true;
        }
        
        public void CreateOn(Vector2 pos)
        {
            var bush = Instantiate(_bushPf, pos, Quaternion.Euler(0,0, 90 * Random.Range(0, 4)), transform);
            bush.Init();
            _bushes.Add(bush);
        }

        public void ClearAll()
        {
            foreach (var bush in _bushes)
            {
                Destroy(bush.gameObject);
            }

            _bushes.Clear();
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