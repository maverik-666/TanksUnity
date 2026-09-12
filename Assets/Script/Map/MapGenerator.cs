namespace Project
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class MapGenerator : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField]
        private MapConfig _config;
        
        [Header("Tile maps")]
        [SerializeField]
        private Tilemap _floorTilemap = null;
             
        [SerializeField]
        private Tilemap _wallTilemap = null;
        
        [SerializeField]
        private Tile[] _floorTiles;
   
        [SerializeField]
        private Tile[] _wallTiles;
        
        [Header("Prefabs")]
        [SerializeField]
        private EdgeCollider2D _borderPf;
        
        [SerializeField]
        private SpawnPoint _spawnPf;
        
        private Dictionary<int, IContentFactory> _contentFactories = null;
        private Vector2Int[] _occupied;
        
        private readonly Vector2Int[] _offsets = 
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 1),
        };

        private Registry<SpawnPoint> _spawnPointRegistry = null; 
        private CameraBoundsUpdateHandler _cameraBounds = null;
        private bool _isInitialized = false;

        private Vector2Int Size => _config.Size;
        
        public void Init(CameraBoundsUpdateHandler cameraBounds, params IContentFactory[] contentFactories)
        {
            _spawnPointRegistry = this.Resolve<Registry<SpawnPoint>>();
            _cameraBounds = cameraBounds;
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
            
            SetupCameraBounds();
            
            GenerateBorders();
            GenerateFloorAndWalls();

            GenerateSpawns();
            
            GenerateContent();
        }

        private void SetupCameraBounds()
        {
            var bounds = new Bounds(_floorTilemap.layoutGrid.transform.position + new Vector3(Size.x/2f, Size.y/2f), (Vector2) Size);
            _cameraBounds.SetBounds(bounds);
        }
        
        private void GenerateBorders()
        {
            var wall = Instantiate(_borderPf, Vector3.one * -0.5f, Quaternion.identity);
            wall.points = new Vector2[]
            { 
                _wallTilemap.layoutGrid.GetCellCenterWorld(new Vector3Int(0, 0, 0)), 
                _wallTilemap.layoutGrid.GetCellCenterWorld(new Vector3Int(Size.x, 0, 0)), 
                _wallTilemap.layoutGrid.GetCellCenterWorld(new Vector3Int(Size.x, Size.y, 0)),
                _wallTilemap.layoutGrid.GetCellCenterWorld(new Vector3Int(0, Size.y, 0)), 
                _wallTilemap.layoutGrid.GetCellCenterWorld(new Vector3Int(0, 0, 0))
            };
        }
        
        private void GenerateFloorAndWalls()
        {
            for (int y = -1; y < Size.y + 1; y++)
            for (int x = -1; x < Size.x + 1; x++)
            {
                if (y < 0 || x < 0 || y >= Size.y || x >= Size.x)
                {
                    _wallTilemap.SetTile(new Vector3Int(x, y, 0), _wallTiles[Random.Range(0, _wallTiles.Length)]);
                }
                else
                {
                    _floorTilemap.SetTile(new Vector3Int(x, y, 0), _floorTiles[Random.Range(0, _floorTiles.Length)]);
                }
            }
        }
        
        private void GenerateSpawns()
        {
            _occupied = new Vector2Int[_config.SpawnCount];
            
            for (int i = 0; i < _occupied.Length; i++)
            {
                Vector2Int spawnPos;
                
                do
                { 
                    spawnPos = new Vector2Int(Random.Range(1, Size.x -1), Random.Range(1, Size.y - 1));
                } 
                while (IsOccupied(spawnPos));
                
                _occupied[i] = spawnPos;
                var spawnPoint = Instantiate(_spawnPf, CalcWorldPosFor(spawnPos), Quaternion.identity, transform);
                _spawnPointRegistry.Add(spawnPoint);
            }
        }

        private bool IsOccupied(Vector2Int pos)
        {
            foreach (var occupied in _occupied)
            {
                if (occupied.x == pos.x && occupied.y == pos.y)
                {
                    return true;
                }

                foreach (var offset in _offsets)
                {
                    if (occupied.x == pos.x + offset.x && occupied.y == pos.y + offset.y)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void GenerateContent()
        {
            for (int y = 0; y < Size.y; y++) 
            for (int x = 0; x < Size.x; x++)
            {
                if (IsOccupied(new Vector2Int(x, y)))
                {
                    continue;
                }
                
                int contentId = GetContentId();
                if (contentId == -1)
                {
                    continue;
                }

                _contentFactories[contentId].CreateOn(CalcWorldPosFor(x, y));
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

        private Vector2 CalcWorldPosFor(Vector2Int pos)
        {
            return _floorTilemap.GetCellCenterWorld((Vector3Int) pos);
        } 
        
        private Vector2 CalcWorldPosFor(int x, int y)
        {
            return _floorTilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));
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