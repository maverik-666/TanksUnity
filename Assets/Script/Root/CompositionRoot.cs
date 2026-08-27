namespace Project
{
    using UnityEngine;
    
    public class CompositionRoot : MonoBehaviour
    {

        public void Awake()
        {
            var camera = GetComponentInChildren<Camera>(true);
            var inputSystem = GetComponentInChildren<InputSystem>(true);
     
            inputSystem.Init();

        }
    }
}