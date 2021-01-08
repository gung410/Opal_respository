using System.Threading.Tasks;

namespace Conexus.Opal.InboxPattern.Infrastructure
{
    public interface IInboxQueue
    {
        Task QueueMessageAsync(QueueMessage queueMessage);
    }
}
