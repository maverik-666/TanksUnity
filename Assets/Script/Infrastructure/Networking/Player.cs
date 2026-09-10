namespace Project
{
    using Unity.Netcode;
    using UnityEngine;

    public class Player : NetworkBehaviour
    {
        private Context _context;
        private Tank _tank = null;
        private OwnerPlayerRegistry _registry;

        private readonly NetworkVariable<Vector2> _tankPosition = new NetworkVariable<Vector2>();
        private readonly NetworkVariable<float> _tankRotation = new NetworkVariable<float>();
        private readonly NetworkVariable<float> _turretRotation = new NetworkVariable<float>();
        
        public override void OnNetworkSpawn()
        {
            _context = CompositionRoot.Instance;
            var tankFactory = _context.Resolve<TankFactory>();

            var position = transform.position + Vector3.right * (2 * OwnerClientId);
            
            _tank = tankFactory.Create(position);
            
            if (IsOwner)
            {
                _registry = _context.Resolve<OwnerPlayerRegistry>();
                _registry.Add(this);
                SpawnRpc(position);
            }
        }

        public void Shoot()
        {
            ShootRpc();
        }

        [Rpc(SendTo.Server)]
        private void ShootRpc()
        {
            if (_tank.CanShoot)
            {
                _tank.Shoot(false);
                ShootValidatedRpc();
            }
        }
        
        [Rpc(SendTo.NotServer)]
        private void ShootValidatedRpc()
        {
            _tank.Shoot(true);
        }

        [Rpc(SendTo.Server)]
        private void SpawnRpc(Vector3 position)
        {
            _tankPosition.Value = _tank.transform.position = position;
        }
        
        public void Move(Vector2 input, float deltaTime)
        {
            _tank.Move(input, deltaTime);
            MoveRpc(input, deltaTime);
        }
        
        [Rpc(SendTo.Server)]
        private void MoveRpc(Vector2 input, float deltaTime)
        {
            _tank.Move(input, deltaTime);
            
            _tankPosition.Value = _tank.Position;
            _tankRotation.Value = _tank.Rotation.eulerAngles.z;
        }

        public void Rotate(Vector2 targetPos, float deltaTime)
        {
            _tank.Rotate(targetPos, deltaTime); 
            RotateRpc(targetPos, deltaTime);
        }
        
        [Rpc(SendTo.Server)]
        private void RotateRpc(Vector2 targetPos, float deltaTime)
        {
            _tank.Rotate(targetPos, deltaTime); 
            _turretRotation.Value = _tank.Turret.Rotation.eulerAngles.z;
        }
        
        private void Update()
        {
            if (IsClient)
            {
                _tank.transform.position = Vector3.Lerp(_tank.Position, _tankPosition.Value, 0.1f);
                _tank.transform.rotation = Quaternion.Lerp(_tank.Rotation, Quaternion.Euler(0 ,0 , _tankRotation.Value), 0.1f);
                _tank.Turret.transform.rotation = Quaternion.Lerp(_tank.Turret.Rotation, Quaternion.Euler(0, 0, _turretRotation.Value), 0.1f);
            }
        }
        
        public override void OnNetworkDespawn()
        {
            if (IsOwner) 
            { 
                _registry.Remove(this);
            }
            
            Destroy(_tank.gameObject);
            
            base.OnNetworkDespawn();
        }
    }
}