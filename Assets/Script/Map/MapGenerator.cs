namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField]
        private float _boxProb = 0.3f;

        [SerializeField]
        private float _gasolineProb = 0.3f;
        
        private Vector2Int _size;
        private bool _isInitialized = false;

        private int[][] _map;

        public void Init(Vector2Int size)
        {
            _size = size;
            _isInitialized = true;
        
            _map = new int[_size.x][];
            for (int i = 0; i < _map.Length; i++)
            {
                _map[i] = new int[_size.y];
            }
        }

        public void Generate()
        {
            for (int y = 0; y < _map.Length; y++) 
            for (int x = 0; x < _map[y].Length; x++)
            {
                _map[y][x] = ;
            }
        }
        

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(MapGenerator)} not initialized", this);
            }
        }
#endif
    }
}