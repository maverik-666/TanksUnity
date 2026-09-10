namespace Project
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "MapConfig", menuName = "ScriptableObjects/MapConfig")]
    public class MapObject : ScriptableObject
    { 
        [SerializeField] 
        private int _id;

        public int ID => _id;
    }
}