namespace Project
{
    public interface IUpdateHandler
    {
        
        public bool Enabled { get; }
        
        public void UpdateTick(float deltaTime);
    }
}