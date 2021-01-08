using System.Threading.Tasks;

namespace Microservice.WebinarAutoscaler.Application.Services
{
    public interface IBBBSyncService
    {
        /// <summary>
        /// To guarantee that got 1 available server.
        /// </summary>
        /// <returns>Task.Completed.</returns>
        Task ScaleOutBBBServerAsync();

        /// <summary>
        /// To remove redundant servers.
        /// </summary>
        /// <returns>Task.Completed.</returns>
        Task ScaleInBBBServerAsync();

        /// <summary>
        /// To find BBB server that enough room for meetings.
        /// </summary>
        /// <returns>Task.Completed.</returns>
        Task SeekCoordinatedBBBServerAsync();
    }
}
