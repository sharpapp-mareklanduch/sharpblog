namespace SharpBlog.Database.Models
{
    public abstract class Entity<T> : IEntity
    {
        public T Id { get; set; }
    }
}
