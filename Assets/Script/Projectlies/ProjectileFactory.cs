namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class ProjectileFactory : MonoBehaviour, IProjectileFactory
    {
        [SerializeField]
        private Projectile _projectilePf;

        private Registry<Projectile> _projectiles = null;
        private bool _isInitialized = false;

        public void Init(Registry<Projectile> projectiles)
        {
            _projectiles = projectiles;
            _isInitialized = true;
        }

        public void Create(Tank owner, Vector3 position, int damage, Quaternion angle)
        {
            var projectile = Instantiate(_projectilePf, position, angle, transform);
            projectile.Init(owner, damage, angle * Vector2.right);
            _projectiles.Add(projectile);
        }
        
        public void Dispose(Projectile projectile)
        {
            _projectiles.Remove(projectile);
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
