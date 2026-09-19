namespace Project
{
    public class PlayerRegistry : Registry<IPlayer>
    {
        public PlayerRegistry(int capacity) : base(capacity) {}
    }
}