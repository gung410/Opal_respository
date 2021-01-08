using Thunder.Platform.Cqrs;

namespace Microservice.NewsFeed.Application.Commands
{
    public class MigrateUserCommand : BaseThunderCommand
    {
        public int BatchSize { get; set; }
    }
}
