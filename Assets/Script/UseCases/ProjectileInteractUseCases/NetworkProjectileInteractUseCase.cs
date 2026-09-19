namespace Project
{
    using Unity.Netcode;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class NetworkProjectileInteractUseCase : NetworkBehaviour, IProjectileInteractUseCase
    {
        [SerializeField]
        private OneShotVFXId _oneShotVFXId;

        [SerializeField]
        private float _forcePower = 20;

        private IndexedRegistry<Projectile> _projectileRegistry = null;
        private IndexedRegistry<IInteractable> _interactableRegistry = null;
        private NetworkProjectileFactory _projectileFactory = null;
        private OneShotVFXFactory _oneShotVFXFactory = null;
        private bool _isInitialized = false;

        public void Init(IndexedRegistry<Projectile> projectileRegistry, IndexedRegistry<IInteractable> interactableRegistry, NetworkProjectileFactory projectileFactory, OneShotVFXFactory oneShotVFXFactory)
        {
            _projectileRegistry = projectileRegistry;
            _interactableRegistry = interactableRegistry;
            _projectileFactory = projectileFactory;
            _oneShotVFXFactory = oneShotVFXFactory;
            _isInitialized = true;
        }

        public void Resolve(Projectile projectile, Collider2D other)
        {
            if (!IsServer)
                return;

            if (other.transform == projectile.Owner.transform)
                return;

            int interactableId = -1;
            
            if (other.TryGetComponent(out IInteractable interactable))
            {
                interactableId = _interactableRegistry.GetIdBy(interactable);
                interactable.Interact(projectile);
            }

            if (other.TryGetComponent(out Rigidbody2D rigidBody))
                rigidBody.AddForce(projectile.Direction * _forcePower, ForceMode2D.Impulse);

            int projectileId = _projectileRegistry.GetIdBy(projectile);
            Vector2 pos = projectile.Position;
            
            _projectileFactory.Dispose(projectile);
            _oneShotVFXFactory.Create(_oneShotVFXId, pos);

            ResolveRpc(projectileId, pos, interactableId);
        }

        [Rpc(SendTo.NotServer)]
        private void ResolveRpc(int projectileId, Vector2 position, int interactableId)
        {
            var projectile = _projectileRegistry.GetItemBy(projectileId);

            if (interactableId != -1)
            {
                _interactableRegistry.GetItemBy(interactableId).Interact(projectile);
            }
            
            _projectileFactory.Dispose(projectile);
            _oneShotVFXFactory.Create(_oneShotVFXId, position);
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(NetworkProjectileInteractUseCase)} not initialized", this);
            }
        }
#endif
    }
}
