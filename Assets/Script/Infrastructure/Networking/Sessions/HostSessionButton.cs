namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class HostSessionButton : MonoBehaviour, IInteractable 
    {
        private SessionConnector _sessionConnector;

        public void Init(SessionConnector sessionConnector)
        {
            _sessionConnector = sessionConnector;
        }

        public void Interact(Projectile projectile)
        {
            _sessionConnector.RequestHost();
        }
    }
}