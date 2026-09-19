namespace Project
{
    using TMPro;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class CounterSetting : MonoBehaviour
    {
        [Header("Elements")]
        [SerializeField]
        private SettingButton _upButton = null;

        [SerializeField]
        private SettingButton _downButton = null;
        
        [SerializeField]
        private TMP_Text _valueText = null;
        
        [Header("Counter")]
        [SerializeField]
        private int _interactFactor = 1;
        
        [SerializeField]
        private int _min = 0;
        
        [SerializeField]
        private int _max = 10;

        [SerializeField]
        private int _initialValue = 0;
        
        private int _value = 0;
        
        private bool _isInitialized = false;

        public int Value => _value;

        public void Init(InteractableRegistrator interactableRegistrator)
        {
            _upButton.Init(this);
            _downButton.Init(this);
            
            interactableRegistrator.Register(_upButton);
            interactableRegistrator.Register(_downButton);

            _value = _initialValue;
            
            UpdateText();
            
            _isInitialized = true;
        }
        
        public void OnInteractWith(SettingButton settingButton)
        {
            if (settingButton == _upButton)
            {
                _value = Mathf.Min(_value + _interactFactor, _max);
            }
            else if (settingButton == _downButton)
            {
                _value = Mathf.Max(_value - _interactFactor, _min);
            }
            
            UpdateText();
        }
        
        private void UpdateText()
        {
            _valueText.text = _value.ToString();
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(CounterSetting)} not initialized", this);
            }
        }
#endif
    }
}