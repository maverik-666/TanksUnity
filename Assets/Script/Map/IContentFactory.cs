namespace Project
{
    using UnityEngine;

    public interface IContentFactory
    {
        public MapContentId ContentId { get; }
        
        public void CreateOn(Vector2 pos);
    }
}