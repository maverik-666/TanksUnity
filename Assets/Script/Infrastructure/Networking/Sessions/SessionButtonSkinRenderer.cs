namespace Project
{
    using TMPro;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class SessionButtonSkinRenderer : MonoBehaviour
    {
        private string _sessionName;
        private GameState _gameState;
        
        private SpriteRenderer _spriteRenderer = null;
        private TMP_Text _titleText = null;
        
        private bool _isInitialized = false;

        public void Init(string sessionName, GameState gameState)
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
            _titleText = GetComponentInChildren<TMP_Text>(true);

            _sessionName = sessionName;
            _gameState = gameState;
            
            FormatText();
            
            _isInitialized = true;
        }

        public void UpdateState(GameState gameState)
        {
            _gameState = gameState;
            FormatText();
        }
        
        private void FormatText()
        {
            string stateText = _gameState == GameState.Wait ? "waiting" : "in game";
            _titleText.SetText($"{_sessionName}\n{stateText}");
        }
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(SessionButtonSkinRenderer)} not initialized", this);
            }
        }
#endif
       
    }
}