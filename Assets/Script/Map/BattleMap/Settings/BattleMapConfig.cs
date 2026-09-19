namespace Project
{
    using Unity.Netcode;
    using UnityEngine;

    public struct BattleMapConfig : INetworkSerializable
    {
        public Vector2Int Size;
        public ContentChance[] Content;
        public int SpawnCount;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref  Size);
            serializer.SerializeValue(ref Content);
            serializer.SerializeValue(ref  SpawnCount);
        }
    }

    public struct ContentChance : INetworkSerializable
    {
        public int ContentId;
        public float Chance;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref  ContentId);
            serializer.SerializeValue(ref  Chance);

        }
    }
}