namespace Project
{
    using System.Collections.Generic;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField]
        private MapConfig _config;

        [SerializeField]
        private Collider2D _wallPf;

        [SerializeField]
        private SpriteRenderer _floorPf;
        
        private Vector2Int Size => _config.Size;

        private Dictionary<int, IContentFactory> _contentFactories = null;
        private Vector2Int[] _spawns;
 
        private bool _isInitialized = false;

        public void Init(params IContentFactory[] contentFactories)
        {
            _contentFactories = new Dictionary<int, IContentFactory>();
            foreach (var factory in contentFactories)
            {
                _contentFactories.Add(factory.ContentId.ID, factory);
            }
            
            _isInitialized = true;
        }
        
        public void Generate(int seed)
        {
            Random.InitState(seed);
            
            GenerateBorders();
            GenerateFloor();

            GenerateSpawns();
            
            GenerateContent();
        }
        
        private void GenerateBorders()
        {
            for (int i = 0; i < 4; i++)
            {
                var wall = Instantiate(_wallPf, transform.position, Quaternion.identity);

                switch (i)
                {
                    case 0:
                        wall.transform.position = Vector2.right * (Size.x / 2f);
                        wall.transform.localScale = new Vector3(1, Size.y, 1f);
                        break;
                    case 1:
                        wall.transform.position = Vector2.left * (Size.y / 2f);
                        wall.transform.localScale = new Vector3(1, Size.y, 1f);
                        break;
                    case 2:
                        wall.transform.position = Vector2.up * (Size.y / 2f);
                        wall.transform.localScale = new Vector3(Size.x, 1, 1f);
                        break;
                    case 3:
                        wall.transform.position = Vector2.down * (Size.y / 2f);
                        wall.transform.localScale = new Vector3(Size.x, 1, 1f);
                        break;
                }
            }
        }
        
        private void GenerateFloor()
        {
            var floor = Instantiate(_floorPf, transform.position, Quaternion.identity);
            floor.transform.localScale = new Vector3(Size.x, Size.y, 1);
            
        }
        
        private void GenerateSpawns()
        {
            _spawns = new Vector2Int[_config.SpawnCount];
            
            for (int i = 0; i < _spawns.Length; i++)
            {
                Vector2Int spawnPos;
                
                do
                { 
                    spawnPos = new Vector2Int(Random.Range(1, Size.x -1), Random.Range(1, Size.y - 1));
                } 
                while (IsOccupied(spawnPos));
                
                _spawns[i] = spawnPos;
            }
        }

        private bool IsOccupied(Vector2Int pos)
        {
            foreach (var spawn in _spawns)
            {
                if (spawn.x == pos.x && spawn.y == pos.y)
                {
                    return true;
                }
            }

            return false;
        }

        private void GenerateContent()
        {
            for (int y = 1; y < Size.y-1; y++) 
            for (int x = 1; x < Size.x-1; x++)
            {
                var contentId = GetContentId();

                if (contentId == -1)
                {
                    continue;
                }

                _contentFactories[contentId].CreateOn(x, y);
            }
        }

        private int GetContentId()
        {
            float random = Random.Range(0, 1f);

            for (int i = 0; i < _config.Content.Length; i++)
            {
                float accumulatedChance = 0f;
                
                for (int j = 0; j < i + 1; j++)
                {
                    accumulatedChance += _config.Content[j].Chance;
                }
                
                if (random <= accumulatedChance)
                {
                    return _config.Content[i].ID;
                }
            }
            
            return -1;
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