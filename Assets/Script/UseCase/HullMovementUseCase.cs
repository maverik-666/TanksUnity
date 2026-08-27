namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class HullMovementUseCase : MonoBehaviour
    {
    
        private bool _isInitialized = false;
        
        public void Init()
        {
            _isInitialized = true;
        }

        public void Execute(Hull hull, Vector2 input, float deltaTime)
        {
            if(input.y == 0)
                return;
            
            if (input.y > 0)
                input.x *= -1;
            
            UpdateLinearVelocityOf(hull.Rigidbody2D, hull.HullStats.MovementSpeed, input.y, deltaTime);
            UpdateAngularVelocityOf(hull.Rigidbody2D, hull.HullStats.Agility, input.x, deltaTime);
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
                Debug.LogError($"{nameof(HullMovementUseCase)} not initialized", this);
            }
        }
#endif
    }
}