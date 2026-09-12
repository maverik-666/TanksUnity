namespace Project
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class InputUpdateHandler : MonoBehaviour, IUpdateHandler
    {
        [SerializeField]
        private InputActionReference _positionActionReference;
        
        [SerializeField] 
        private InputActionReference _moveActionReference;
        
        [SerializeField]
        private InputActionReference _shootActionReference;
        
        private Vector2 _mouseScreenPosition = Vector2.zero;
        private Vector2 _moveInput = Vector2.zero;

        private ClickListener _shootListener = null;
        
        
        public Vector2 MoveInput => _moveInput;
        public Vector2 MouseScreenPosition => _mouseScreenPosition;

        public ClickListener ShootListener => _shootListener;

        public bool Enabled => enabled;
        
        public void Init()
        {
            _positionActionReference.action.Enable();
            _moveActionReference.action.Enable();
            
            _shootListener = new ClickListener(_shootActionReference);
        }
        
        public void UpdateTick(float deltaTime)
        {
            _mouseScreenPosition = _positionActionReference.action.ReadValue<Vector2>();
            _moveInput = _moveActionReference.action.ReadValue<Vector2>();
        }

    
        private void OnDestroy()
        {
            _positionActionReference.action.Disable();
            _moveActionReference.action.Disable();
            
            _shootListener.Dispose();
        }
    }
}