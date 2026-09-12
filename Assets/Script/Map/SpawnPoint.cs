namespace Project
{
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    public class SpawnPoint : MonoBehaviour
    {
        public Vector3 Position => transform.position;
    }
}