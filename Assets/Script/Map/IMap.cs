namespace Project
{
    using System.Collections.Generic;
    using UnityEngine;

    public interface IMap
    { 
        public Bounds Bounds { get; }
        
        public SpawnPoint GetSpawnPoint();
    }
}