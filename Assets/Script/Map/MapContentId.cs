namespace Project
{
    using UnityEngine;

    [CreateAssetMenu()]
    public class MapContentId : ScriptableObject
    {
        [SerializeField]
        private int _id;

        public int ID => _id;
    }
}