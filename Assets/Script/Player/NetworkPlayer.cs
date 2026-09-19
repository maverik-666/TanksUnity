namespace Project
{
    using System.Collections.Generic;
    using Unity.Netcode;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class NetworkPlayer : NetworkBehaviour, IPlayer
    {
        private const float InterpolationDelay = 0.1f;

        private struct Snapshot
        {
            public Vector2 Position;
            public float TankRotation;
            public float Timestamp;
        }

        private Context _context = null;
        private Tank _tank = null;
        private OwnerPlayerRegistry _ownerRegistry = null;
        private PlayerPermissions _permissions = PlayerPermissions.Restricted;
        
        private IndexedRegistry<IDamageable> _damageableRegistry = null;
        private AllTankRegistry _allTankRegistry = null;
        private PlayerRegistry _playerRegistry = null;

        private readonly NetworkVariable<Vector2> _tankPosition = new NetworkVariable<Vector2>();
        private readonly NetworkVariable<float> _tankRotation = new NetworkVariable<float>();
        private readonly NetworkVariable<float> _turretRotation = new NetworkVariable<float>();

        private readonly Queue<Snapshot> _snapshots = new Queue<Snapshot>();

        public Vector2 TankPosition => _tank.Position;
        
        public PlayerPermissions Permissions => _permissions;

        public override void OnNetworkSpawn()
        {
            _context = NetworkCompositionRoot.Instance;

            var tankFactory = _context.Resolve<TankFactory>();
            int id = (int)NetworkObjectId + 10000;
            
            _damageableRegistry = _context.Resolve<IndexedRegistry<IDamageable>>();
            _allTankRegistry = _context.Resolve<AllTankRegistry>();
            _playerRegistry = _context.Resolve<PlayerRegistry>();
            
            if (IsServer)
            {
                var activeMap = _context.Resolve<ActiveMapRegistry>();
                var spawnPoints = activeMap.Value.SpawnPoints;
                var position = spawnPoints[Random.Range(0, spawnPoints.Count)].Position;  
                _tank = tankFactory.Create(this, position);
                _tankPosition.Value = position;
                
                if (IsOwner)
                {
                    RegisterAsOwner();

                    if (IsHost)
                    {
                        _permissions = PlayerPermissions.All;
                    }
                }
            }
            else
            {
                _tank = tankFactory.Create(this, _tankPosition.Value);

                if (IsOwner)
                {
                    RegisterAsOwner();
                }
                else
                {
                    _tankPosition.OnValueChanged += OnSnapshotReceived;
                }
            }
            
            _damageableRegistry.Add(id, _tank); 
            _allTankRegistry.Add(id, _tank);
            _playerRegistry.Add(this);
        }

        private void OnSnapshotReceived(Vector2 prev, Vector2 next)
        {
            _snapshots.Enqueue(new Snapshot
            {
                Position = next,
                TankRotation = _tankRotation.Value,
                Timestamp = Time.time
            });
        }
        
        private void RegisterAsOwner()
        {
            _ownerRegistry = _context.Resolve<OwnerPlayerRegistry>();
            _ownerRegistry.Set(this);
            _context.Resolve<CameraFollower>().Teleport();
            _context.Resolve<PlayerHealthBar>().SetTarget(_tank.Health);
        } 

        public void Shoot()
        {
            if (_tank.IsDead)
                return;

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

        public void Move(Vector2 input, float deltaTime)
        {
            if (_tank.IsDead)
                return;

            _tank.Move(input, deltaTime);
            MoveRpc(_tank.Position, _tank.Rotation.eulerAngles.z);
        }

        [Rpc(SendTo.Server)]
        private void MoveRpc(Vector2 position, float rotation)
        {
            if (!IsOwner)
            {
                _tank.transform.position = position;
                _tank.transform.rotation = Quaternion.Euler(0, 0, rotation);
            }

            _tankPosition.Value = position;
            _tankRotation.Value = rotation;
        }

        public void Rotate(Vector2 targetPos, float deltaTime)
        {
            if (_tank.IsDead)
                return;

            _tank.Rotate(targetPos, deltaTime);
            RotateRpc(targetPos, deltaTime);
        }
        
        [Rpc(SendTo.Server)]
        private void RotateRpc(Vector2 targetPos, float deltaTime)
        {
            if (!IsOwner)
            {
                _tank.Rotate(targetPos, deltaTime);
            }

            _turretRotation.Value = _tank.Turret.Rotation.eulerAngles.z;
        }
        
        public void Teleport(Vector2 targetPosition)
        {
            _tank.gameObject.SetActive(false);
            _tank.transform.position = targetPosition;
            _tank.gameObject.SetActive(true);
        
            TeleportRpc(targetPosition);
            _tankPosition.Value = targetPosition;
        }
        
        [Rpc(SendTo.NotServer)]
        private void TeleportRpc(Vector2 targetPosition)
        {
            _tank.gameObject.SetActive(false);
            _tank.transform.position = targetPosition;
            _tank.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (!IsClient || IsOwner)
            {
                return;
            }

            InterpolatePosition();
            InterpolateTurret();
        }

        private void InterpolatePosition()
        {
            float renderTime = Time.time - InterpolationDelay;

            while (_snapshots.Count > 2 && _snapshots.Peek().Timestamp < renderTime)
            {
                _snapshots.Dequeue();
            }

            var buffer = _snapshots.ToArray();
            if (buffer.Length < 2)
            {
                return;
            }

            float t = Mathf.InverseLerp(buffer[0].Timestamp, buffer[1].Timestamp, renderTime);
            _tank.transform.position = Vector2.Lerp(buffer[0].Position, buffer[1].Position, t);
            _tank.transform.rotation = Quaternion.Lerp(
                Quaternion.Euler(0, 0, buffer[0].TankRotation),
                Quaternion.Euler(0, 0, buffer[1].TankRotation),
                t);
        }

        private void InterpolateTurret()
        {
            _tank.Turret.transform.rotation = Quaternion.Lerp(
                _tank.Turret.Rotation,
                Quaternion.Euler(0, 0, _turretRotation.Value),
                Time.deltaTime * 20f);
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner)
            {
                _tankPosition.OnValueChanged -= OnSnapshotReceived;
            }

            if (IsOwner)
            {
                _ownerRegistry.Unset(this);
            }
            
            _damageableRegistry.Remove(_tank);
            _allTankRegistry.Remove(_tank);
            _playerRegistry.Remove(this);

            Destroy(_tank.gameObject);

            base.OnNetworkDespawn();
        }
    }
}
