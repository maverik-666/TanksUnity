namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class GasolineFactory : MonoBehaviour, IContentFactory
    {
        [SerializeField]
        private MapContentId _contentId = null;
        
        [SerializeField]
        private Destructible _gasolinePf = null;
        
        private bool _isInitialized = false;

        public MapContentId ContentId => _contentId;
        
        public void Init()
        {
            _isInitialized = true;
        }
        
        public void CreateOn(int x, int y)
        {
            var gasoline = Instantiate(_gasolinePf, new Vector3(x,y,0), Quaternion.identity, transform);
            gasoline.Init();
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