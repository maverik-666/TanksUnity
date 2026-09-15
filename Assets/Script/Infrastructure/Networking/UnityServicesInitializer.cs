namespace Project
{
    using System;
    using System.Threading.Tasks;
    using Unity.Services.Authentication;
    using UnityEngine;
    using Unity.Services.Core;

    public class UnityServicesInitializer : MonoBehaviour
    {
        public async Task Init()
        {
            if (UnityServices.Instance.State == ServicesInitializationState.Initialized)
            {
                return;
            }
            
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Sighed in! MY id is: {AuthenticationService.Instance.PlayerId}");
        }
    }
}