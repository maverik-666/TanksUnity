namespace Project
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class InputSystem : MonoBehaviour
    {
        [SerializeField]
        private InputActionReference _positionActionReference;
        
        [SerializeField] 
        private InputActionReference _axisActionReference;
        
        [SerializeField]
        private InputActionReference _leftClickActionReference;
        
        [SerializeField]
        private InputActionReference _rightClickActionReference;
        
        private Vector2 _mousePosition = Vector2.zero;
        private Vector2 _inputAxis = Vector2.zero;

        private ClickListener _leftClickListener = null;
        private ClickListener _rightClickListener = null;
        
        
        public Vector2 InputAxis => _inputAxis;
        public Vector2 MousePosition => _mousePosition;

        public ClickListener LeftClickListener => _leftClickListener;

        public ClickListener RightClickListener => _rightClickListener;

        public void Init()
        {
            _positionActionReference.action.Enable();
            _axisActionReference.action.Enable();
            
            _leftClickListener = new ClickListener(_leftClickActionReference);
            _rightClickListener = new ClickListener(_rightClickActionReference);
        }

        private void Update()
        {
            _mousePosition = _positionActionReference.action.ReadValue<Vector2>();
            _inputAxis = _axisActionReference.action.ReadValue<Vector2>();
        }

    
        private void OnDestroy()
        {
            _positionActionReference.action.Disable();
            _axisActionReference.action.Disable();
            
            _leftClickListener.Dispose();
            _rightClickListener.Dispose();
        }
    }
}