namespace Project
{
    using System;
    using UnityEngine;

    [Serializable]
    public struct TankStats
    {
        public TurretStats TurretStats;
        
        public float MovementSpeed;
        public float Agility;
        
        public float Health;
    }
}