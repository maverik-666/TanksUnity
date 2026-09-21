namespace Project
{
    using UnityEngine;

    public interface IPlayer
    {
        Vector2 TankPosition { get; }
        
        public Health Health { get; }
        
        void Shoot();
        
        void Move(Vector2 input, float deltaTime);
        
        void Rotate(Vector2 targetPos, float deltaTime);
        
        void Teleport(Vector2 targetPosition);

        void Revive();

        public void SetInvincibility(bool isInvincible);
        
        public PlayerPermissions Permissions { get; }
    }
}