using System.Threading.Tasks;

namespace Conexus.Opal.OutboxPattern
{
    public interface IOutboxQueue
    {
        Task QueueMessageAsync(QueueMessage queueMessage);

        Task RequeueMessageAsync();
    }
}
