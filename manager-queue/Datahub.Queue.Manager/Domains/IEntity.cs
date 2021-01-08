namespace Datahub.Queue.Manager.Domains
{
    public interface IEntity<T>
    {
        T Id { get; set; }
    }
}
