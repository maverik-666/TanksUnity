namespace Project
{
    public interface IContentFactory
    {
        public MapContentId ContentId { get; }
        
        public void CreateOn(int x, int y);
    }
}