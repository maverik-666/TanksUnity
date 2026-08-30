namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class TankMovementUseCase : MonoBehaviour
    {
    
        private bool _isInitialized = false;
        
        public void Init()
        {
            _isInitialized = true;
        }

        public void Execute(Tank tank, Vector2 input, float deltaTime)
        {
            if(input.y == 0)
                return;
            
            if (input.y > 0)
                input.x *= -1;
            
            UpdateLinearVelocityOf(tank.Rigidbody2D, tank.TankStats.MovementSpeed, input.y, deltaTime);
            UpdateAngularVelocityOf(tank.Rigidbody2D, tank.TankStats.Agility, input.x, deltaTime);
        }
        
        private void UpdateLinearVelocityOf(Rigidbody2D tankBody, float speed, float direction, float deltaTime)
        { 
            float movementSpeed = direction < 0 ? speed * 0.75f : speed;
            var addVelocity = new Vector2(movementSpeed * direction * deltaTime,0).Rotate(tankBody.rotation);

            tankBody.linearVelocity += addVelocity;
        }

        private void UpdateAngularVelocityOf(Rigidbody2D tankBody, float agility, float direction, float deltaTime) {
            tankBody.angularVelocity += agility * direction * deltaTime;
        }
        
#if UNITY_EDITOR  
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(TankMovementUseCase)} not initialized", this);
            }
        }
#endif
    }
}