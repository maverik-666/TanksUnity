namespace Project
{
    using System.Collections.Generic;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class GasolineFactory : MonoBehaviour, IContentFactory
    {
        [SerializeField]
        private MapContentId _contentId = null;
        
        [SerializeField]
        private Destructible _gasolinePf = null;

        private List<Destructible> _gasolines = null;
        private InteractableRegistrator _interactableRegistrator = null;
        private bool _isInitialized = false;

        public MapContentId ContentId => _contentId;
        
        public void Init(InteractableRegistrator interactableRegistrator)
        {
            _gasolines = new List<Destructible>();
            _interactableRegistrator = interactableRegistrator;
            _isInitialized = true;
        }
        
        public void CreateOn(Vector2 pos)
        {
            var gasoline = Instantiate(_gasolinePf, pos, Quaternion.Euler(0,0, 45 * Random.Range(0, 8)), transform);
            gasoline.Init(_interactableRegistrator);
            _gasolines.Add(gasoline);
        }

        public void ClearAll()
        {
            foreach (var gasoline in _gasolines)
            {
                if (_interactableRegistrator.HasDestructible(gasoline))
                {
                    _interactableRegistrator.Unregister(gasoline);
                    Destroy(gasoline.gameObject);
                }
            }

            _gasolines.Clear();
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(GasolineFactory)} not initialized", this);
            }
        }
#endif
       
    }
}