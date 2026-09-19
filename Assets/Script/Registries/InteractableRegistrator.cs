namespace Project
{
    public class InteractableRegistrator : IndexedRegistrator<IInteractable>
    {
        public InteractableRegistrator(IndexedRegistry<IInteractable> damageableRegistry) : base(damageableRegistry)
        {
        }
    }
}