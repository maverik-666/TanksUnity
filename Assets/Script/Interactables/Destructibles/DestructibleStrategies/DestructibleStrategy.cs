namespace Project
{
    using UnityEngine;

    [RequireComponent(typeof(Transform))]
    public abstract class DestructibleStrategy : MonoBehaviour
    {
        public abstract void Init(IDamageable damageable);

        public virtual void Destruct() {}
    }
}