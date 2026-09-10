namespace Project
{
    using System;
    using UnityEngine;

    [Serializable]
    public class MapConfig
    {
        [SerializeField]
        private Vector2Int _size = new Vector2Int(25, 25);
        
        [SerializeField]
        private ContentChance[] _content;
        
        [SerializeField]
        private int _spawnCount;

        public Vector2Int Size => _size;

        public ContentChance[] Content => _content;

        public int SpawnCount => _spawnCount;
    }

    [Serializable]
    public class ContentChance
    {
        [SerializeField]
        private int _id;

        [SerializeField]
        private float _chance;
        
        public int ID => _id;
        
        public float Chance => _chance;
    }
}