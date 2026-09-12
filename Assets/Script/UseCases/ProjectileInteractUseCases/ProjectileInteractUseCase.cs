namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class ProjectileInteractUseCase : MonoBehaviour, IProjectileInteractUseCase
    {
        [SerializeField]
        private OneShotVFXId _oneShotVFXId;

        [SerializeField]
        private float _forcePower = 20;
        
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
            if (other.transform == projectile.Owner.transform)
            {
                return;
            }

            if (other.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(projectile);
            }

            if (other.TryGetComponent(out Rigidbody2D rigidBody))
            {
                rigidBody.AddForce(projectile.Direction * _forcePower, ForceMode2D.Impulse);
            }

            _projectileFactory.Dispose(projectile);
            _oneShotVFXFactory.Create(_oneShotVFXId, projectile.Position);
        } 
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(ProjectileInteractUseCase)} not initialized", this);
            }
        }
#endif
    }
}