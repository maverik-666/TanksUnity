namespace Project
{
    public class OwnerPlayerRegistry : Registry<IPlayer>
    {
        public OwnerPlayerRegistry() : base(1)
        {
        }
    }
}