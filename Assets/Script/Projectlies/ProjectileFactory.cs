namespace Project
{
    using Unity.Netcode;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class ProjectileFactory : NetworkBehaviour
    {
        [SerializeField]
        private Projectile _projectilePf;

        private Registry<Projectile> _projectiles = null;
        private IndexedRegistry<Projectile> _indexedProjectiles = null;
        private AllTankRegistry _allTankRegistry = null;
        private int _nextId = 0;
        private bool _isInitialized = false;

        public void Init(Registry<Projectile> projectiles, IndexedRegistry<Projectile> indexedProjectiles, AllTankRegistry allTankRegistry)
        {
            _projectiles = projectiles;
            _indexedProjectiles = indexedProjectiles;
            _allTankRegistry = allTankRegistry;
            _isInitialized = true;
        }

        public void Create(Tank owner, Vector3 position, int damage, Quaternion angle)
        {
            if (!IsServer)
                return;

            int id = _nextId++;
            int ownerId = _allTankRegistry.GetIdBy(owner);

            CreateLocal(id, owner, position, damage, angle);
            CreateRpc(id, ownerId, position, damage, angle);
        }

        [Rpc(SendTo.NotServer)]
        private void CreateRpc(int id, int ownerId, Vector2 position, int damage, Quaternion angle)
        {
            var owner = _allTankRegistry.GetItemBy(ownerId);
            CreateLocal(id, owner, position, damage, angle);
        }

        private void CreateLocal(int id, Tank owner, Vector3 position, int damage, Quaternion angle)
        {
            var projectile = Instantiate(_projectilePf, position, angle, transform);
            projectile.Init(owner, damage, angle * Vector2.right);
            
            _projectiles.Add(projectile);
            _indexedProjectiles.Add(id, projectile);
        }
        
        public void Dispose(Projectile projectile)
        {
            _projectiles.Remove(projectile);
            _indexedProjectiles.Remove(projectile);
            Destroy(projectile.gameObject);
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(ProjectileFactory)} not initialized", this);
            }
        }
#endif
    }
}
