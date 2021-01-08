using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Commands
{
    /// <summary>
    /// Base command to support converting rabbitMq message to Insert/Update/Delete Calendar Event.
    /// </summary>
    public abstract class BaseCalendarCommand : BaseThunderCommand
    {
    }
}
