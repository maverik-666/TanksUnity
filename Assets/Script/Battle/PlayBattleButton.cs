namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class PlayBattleButton : MonoBehaviour, IInteractable
    {
        private BattleCreator _battleCreator;
        private bool _isInitialized = false;

        public void Init(BattleCreator battleCreator)
        {
            _battleCreator = battleCreator;
            _isInitialized = true;
        }
        
        public void Interact(Projectile projectile)
        {
            if( projectile.Owner.Player.Permissions == PlayerPermissions.All)
            {
                _battleCreator.StartBattle();
            }
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"{nameof(PlayBattleButton)} not initialized", this);
            }
        }
#endif
   
    }
}