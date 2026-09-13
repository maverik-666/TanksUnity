namespace Project
{
    using UnityEngine;

    public interface IProjectileFactory
    {
        void Create(Tank owner, Vector3 position, int damage, Quaternion angle);
        
        void Dispose(Projectile projectile);
    }
}