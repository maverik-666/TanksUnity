namespace Project
{
    using DG.Tweening;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class TankSkinRenderer : MonoBehaviour
    {
        private static readonly int SaturationColorId = Shader.PropertyToID("_SaturationColor");
        private static readonly int SaturationPhaseId = Shader.PropertyToID("_SaturationPhase");
        
        [SerializeField]
        private Material _sharedMaterial = null;
        
        [SerializeField]
        private Color _flashColor = Color.red;

        [SerializeField]
        private Color _dimColor = Color.grey;
        
        private SpriteRenderer[] _tankRenderers;

        private Tween _recolorTween = null;
        private Tween _flashTween = null;
        private Material _material;
        private bool _isInitialized = false;
        
        public void Init(Tank tank)
        {
            _material = new Material(_sharedMaterial);

            _tankRenderers = tank.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var spriteRenderer in _tankRenderers)
            {
                spriteRenderer.material = _material;
            }
            
            _isInitialized = true;
        }

        public void Dim()
        {
            Recolor(_dimColor, 1);
        }

        public void Flash(float duration)
        {
            FlashColor(_flashColor, duration, 10);
        }
        
        private void FlashColor(Color color, float duration, int flashCount)
        {
            _material.SetColor(SaturationColorId, color);

            _flashTween?.Kill();
            _flashTween = DOVirtual.Float(0f, 1, 1f / flashCount * 2 * duration, SetFillPhase)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .SetLoops(flashCount * 2, LoopType.Yoyo);

            void SetFillPhase(float phase)
            {
                _material.SetFloat(SaturationPhaseId, phase);
            }
        }
        
        private void Recolor(Color color, float duration)
        {
            _recolorTween?.Kill();
            _recolorTween = DOVirtual.Color(_tankRenderers[0].color, color, duration, SetColor)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);
            
            void SetColor(Color phase)
            {
                foreach (var spriteRenderer in _tankRenderers)
                {
                    spriteRenderer.color = color;
                }
            }
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(TankSkinRenderer)} not initialized", this);
            }
        }
#endif
    }
}