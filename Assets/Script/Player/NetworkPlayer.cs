namespace Project
{
    using Unity.Netcode;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class NetworkPlayer : NetworkBehaviour, IPlayer
    {
        private Context _context = null;
        private Tank _tank = null;
        private OwnerPlayerRegistry _ownerRegistry = null;
        private PlayerPermissions _permissions = PlayerPermissions.Restricted;

        private IndexedRegistry<IInteractable> _interactableRegistry = null;
        private AllTankRegistry _allTankRegistry = null;
        private PlayerRegistry _playerRegistry = null;
        private TankFactory _tankFactory = null;

        private Interpolator _interpolator = null;
        private NetworkVariable<Vector2> _tankPosition = new NetworkVariable<Vector2>();
        private bool _isInitialized = false;
        
        public Vector2 TankPosition => _tank.Position;

        public void SetInvincibility(bool isInvincible)
        {
            _tank.SetInvincibility(isInvincible);
        }

        public PlayerPermissions Permissions => _permissions;
        
        public Health Health => _tank.Health;

        public PlayerState GetCurrentState()
        {
            return new PlayerState()
            {
                Position = _tank.Position,
                TankRotation = _tank.Rotation.eulerAngles.z,
                TurretRotation = _tank.Turret.Rotation.eulerAngles.z,
                Velocity = _tank.LinearVelocity,
                TankAngularVelocity = _tank.AngularVelocity,
                Tick = NetworkManager.NetworkTickSystem.LocalTime.Tick
            };
        }
        
        public override void OnNetworkSpawn()
        {
            _context = NetworkCompositionRoot.Instance;

            var tankFactory = _context.Resolve<TankFactory>();
            int id = (int)NetworkObjectId + 10000;
            
            _interactableRegistry = _context.Resolve<IndexedRegistry<IInteractable>>();
            _allTankRegistry = _context.Resolve<AllTankRegistry>();
            _playerRegistry = _context.Resolve<PlayerRegistry>();
            
            if (IsServer)
            {
                var activeMap = _context.Resolve<ActiveMapRegistry>();
                var spawnPoints = activeMap.Value.SpawnPoints;
                var position = spawnPoints[Random.Range(0, spawnPoints.Count)].Position;  
                _tank = tankFactory.Create(this, position);
                _tankPosition.Value = position;
                
                if (IsHost)
                {
                    _permissions = PlayerPermissions.All;
                }
            }
            else
            {
                _tank = tankFactory.Create(this, _tankPosition.Value);
            }
            
            if (IsOwner)
            {
                RegisterAsOwner();
            }
            else
            {
                _tank.Rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                _interpolator = new Interpolator(this, GetCurrentState());
                _isInitialized = true;
            }

            _tank.SetInvincibility(true);
            _interactableRegistry.Add(id, _tank); 
            _allTankRegistry.Add(id, _tank);
            _playerRegistry.Add(this);
        }
        
        
        private void RegisterAsOwner()
        {
            _ownerRegistry = _context.Resolve<OwnerPlayerRegistry>();
            _ownerRegistry.Set(this);
            _context.Resolve<CameraFollower>().Teleport();
            _context.Resolve<PlayerHealthBar>().SetTarget(_tank.Health);

            NetworkManager.NetworkTickSystem.Tick += OnClientTick;
        }

        private void OnClientTick()
        {
            SendStateToServerRpc(GetCurrentState());
        }

        [Rpc(SendTo.Server)]
        private void SendStateToServerRpc(PlayerState playerState)
        {
            _tankPosition.Value = playerState.Position;

            if (IsHost && !IsOwner)
            {
                _interpolator.AddState(playerState);
            }

            SendStateToClientsRpc(playerState);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void SendStateToClientsRpc(PlayerState playerState)
        {
            if (!IsOwner)
            {
                _interpolator.AddState(playerState);
            }
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
        }

        public void Rotate(Vector2 targetPos, float deltaTime)
        {
            if (_tank.IsDead)
                return;

            _tank.Rotate(targetPos, deltaTime);
        }
        
        public void Teleport(Vector2 targetPosition)
        {
            _tank.gameObject.SetActive(false);
            _tank.transform.position = targetPosition;
            
            if (_isInitialized)
            {
                _interpolator.SetState(new PlayerState
                {
                    Position = targetPosition,
                    TankRotation = 0,
                    TurretRotation = 0,
                    Velocity = Vector2.zero,
                    TankAngularVelocity = 0,
                    Tick = NetworkManager.NetworkTickSystem.LocalTime.Tick,
                });
            }
            
            _tank.Teleport();
            _tank.gameObject.SetActive(true);
            
            TeleportRpc(targetPosition);
        }
        
        [Rpc(SendTo.NotServer)]
        private void TeleportRpc(Vector2 targetPosition)
        {
            _tank.gameObject.SetActive(false);
            _tank.transform.position = targetPosition;
            
            if (_isInitialized)
            {
                _interpolator.SetState(new PlayerState
                {
                    Position = targetPosition,
                    TankRotation = 0,
                    TurretRotation = 0,
                    Velocity = Vector2.zero,
                    TankAngularVelocity = 0,
                    Tick = NetworkManager.NetworkTickSystem.LocalTime.Tick,
                });
            }
            
            _tank.Teleport();
            _tank.gameObject.SetActive(true);
        }

        public void Revive()
        {
            _tank.Revive();
        }

        private void Update()
        {
            if ((IsHost || IsClient) && !IsOwner && _isInitialized)
            {
                _interpolator.Update();
            }
        }

        public void SetState(Vector3 position, float tankRotation, float turretRotation)
        {
            _tank.transform.SetPositionAndRotation(position, Quaternion.Euler(0, 0, tankRotation));
            _tank.UpdatePosition();
            _tank.Turret.transform.rotation = Quaternion.Euler(0, 0, turretRotation);
        }
        
        public override void OnNetworkDespawn()
        {
            if (IsOwner)
            {
                _ownerRegistry.Unset(this);
            }
            
            if (IsClient && IsOwner)
            {
                NetworkManager.NetworkTickSystem.Tick -= OnClientTick;
            }
            
            _interactableRegistry.Remove(_tank);
            _allTankRegistry.Remove(_tank);
            _playerRegistry.Remove(this);

            Destroy(_tank.gameObject);

        }
    }
}
