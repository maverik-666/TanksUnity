namespace Project
{
    using Unity.Netcode;

    public class ServerTickSystem : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            NetworkManager.NetworkTickSystem.Tick += OnTick;
        }

        private void OnTick()
        {
            if (IsServer)
            {
                ServerTick();
            }
            else if (IsClient)
            {
                ClientTick();
            }
        }

        private void ServerTick()
        {
            
        }

        private void ClientTick()
        {
            
        }
        
        public override void OnNetworkDespawn()
        {
            NetworkManager.NetworkTickSystem.Tick -= OnTick;
        }
    }
}