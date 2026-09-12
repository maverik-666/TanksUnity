namespace Project
{
    using UnityEngine;

    public interface IProjectileInteractUseCase
    {
        void Resolve(Projectile projectile, Collider2D other);
    }
}
