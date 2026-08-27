namespace Project
{
    using System;
    using UnityEngine;

    [Serializable]
    public struct TankStats
    {
        public HullStats HullStats;
        public TurretStats TurretStats;
    }
}