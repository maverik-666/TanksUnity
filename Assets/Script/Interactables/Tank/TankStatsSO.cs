namespace Project
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "TankStats", menuName = "SO/TankStats")]
    public class TankStatsSO : ScriptableObject
    {
        [SerializeField]
        private TankStats _tankStats;
        
        public TankStats TankStats => _tankStats;
        
    }
}