namespace Project
{
    using UnityEngine;

    public interface IPlayer
    {
        Vector2 TankPosition { get; }
        
        void Shoot();
        
        void Move(Vector2 input, float deltaTime);
        
        void Rotate(Vector2 targetPos, float deltaTime);
        
        void Teleport(Vector2 targetPosition);

        void Revive();

        public PlayerPermissions Permissions { get; }
    }
}