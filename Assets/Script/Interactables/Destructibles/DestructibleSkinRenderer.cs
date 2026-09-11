namespace Project
{
    using DG.Tweening;
    using UnityEngine;
    
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class DestructibleSkinRenderer : MonoBehaviour
    { 
        private static readonly int SaturationColorId = Shader.PropertyToID("_SaturationColor");
        private static readonly int SaturationPhaseId = Shader.PropertyToID("_SaturationPhase");

        [SerializeField]
        private Sprite[] _spriteVariants = null;
        
        [SerializeField]
        private Color _flashColor = Color.red;
        
        private SpriteRenderer _spriteRenderer;

        private Tween _flashTween = null;
        private MaterialPropertyBlock _mpb;
        private bool _isInitialized = false;
        
        public void Init()
        {
            _mpb = new MaterialPropertyBlock();
            
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = _spriteVariants[Random.Range(0, _spriteVariants.Length)];
            _spriteRenderer.GetPropertyBlock(_mpb);
            
            _isInitialized = true;
        }

        public void Flash(float duration)
        {
            FlashColor(_flashColor, duration, 10);
        }
        
        private void FlashColor(Color color, float duration, int flashCount)
        {
            _spriteRenderer.GetPropertyBlock(_mpb);
            _mpb.SetColor(SaturationColorId, color);
            _spriteRenderer.SetPropertyBlock(_mpb);
            
            _flashTween?.Kill();
            _flashTween = DOVirtual.Float(0f, 1, 1f / flashCount * 2 * duration, SetFillPhase)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .SetLoops(flashCount * 2, LoopType.Yoyo);

            void SetFillPhase(float phase)
            {
                _spriteRenderer.GetPropertyBlock(_mpb);
                _mpb.SetFloat(SaturationPhaseId, phase);
                _spriteRenderer.SetPropertyBlock(_mpb);
            }
        }
        
#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(DestructibleSkinRenderer)} not initialized", this);
            }
        }
#endif
    }
}