namespace Project
{
    public interface IDamageUseCase
    {
        void Execute(IDamageable damageable, int amount);
    }
}