namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class ProjectileCollisionUseCase : MonoBehaviour
    {
        [SerializeField]
        private OneShotVFXId _oneShotVFXId;
        
        private ProjectileFactory _projectileFactory = null;
        private OneShotVFXFactory _oneShotVFXFactory = null;
        private bool _isInitialized = false;

        public void Init(ProjectileFactory projectileFactory, OneShotVFXFactory oneShotVFXFactory)
        {
            _projectileFactory = projectileFactory;
            _oneShotVFXFactory = oneShotVFXFactory;
            
            _isInitialized = true;
        }

        public void Resolve(Projectile projectile, Collider2D other)
        {
            if (other.transform == projectile.Owner.Hull.transform)
            {
                return;
            }

            if (other.gameObject.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(projectile);
            }

            _projectileFactory.Dispose(projectile);
            _oneShotVFXFactory.Emit(_oneShotVFXId, projectile.Position);
        } 
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(ProjectileCollisionUseCase)} not initialized", this);
            }
        }
#endif
    }
}