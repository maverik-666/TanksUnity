namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class ProjectileUpdateHandler : MonoBehaviour
    {
        private Registry<Projectile> _projectiles = null;
        private bool _isInitialized = false;

        public void Init(Registry<Projectile> projectiles)
        {
            _projectiles = projectiles;
            _isInitialized = true;
        }

        public void UpdateTick(float deltaTime)
        {
            foreach (var projectile in _projectiles.Items)
            {
                projectile.UpdateTick(deltaTime);
            }
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(ProjectileUpdateHandler)} not initialized", this);
            }
        }
#endif
    }
}