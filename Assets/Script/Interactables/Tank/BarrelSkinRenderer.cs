namespace Project
{
    using DG.Tweening;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class BarrelSkinRenderer : MonoBehaviour
    {
        [Header("Barrel Sprites")]
        [SerializeField]
        private Sprite _enabledBarrel;

        [SerializeField]
        private Sprite _disabledBarrel;

        private Sequence _shootSeq = null;
        private SpriteRenderer _spriteRenderer = null;

        private bool _isInitialized = false;

        public void Init()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

            _isInitialized = true;
        }

        public void AnimateShoot()
        {
            _shootSeq?.Kill();
            _shootSeq = DOTween.Sequence();
            _shootSeq.SetLink(gameObject, LinkBehaviour.KillOnDisable);
            _shootSeq.Append(transform.DOLocalMoveX(-0.3f, 0.1f).SetEase(Ease.OutQuad));
            _shootSeq.Append(transform.DOLocalMoveX(0, 0.2f).SetEase(Ease.InQuad));
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(BarrelSkinRenderer)} not initialized", this);
            }
        }
#endif
    }
}