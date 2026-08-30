namespace Project
{
    using UnityEngine;

    public interface IDamageable
    {
        public Vector2 Position { get; }
        
        public Health Health { get; }

        public void TakeDamage();

        public void Die();

        public bool IsDead => Health.IsDead;
        
        public bool CanNotTakeDamage { get; }
    }
}