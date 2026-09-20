namespace Project
{
    using Unity.Netcode;
    using UnityEngine;

    public struct PlayerState : INetworkSerializable
    {
        public Vector2 Position;
        public float TankRotation;
        public float TurretRotation;
        public Vector2 Velocity;
        public float TankAngularVelocity;
        public int Tick;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Position);
            serializer.SerializeValue(ref TankRotation);
            serializer.SerializeValue(ref TurretRotation);
            serializer.SerializeValue(ref Velocity);
            serializer.SerializeValue(ref TankAngularVelocity);
            serializer.SerializeValue(ref Tick);
        }
    }
}