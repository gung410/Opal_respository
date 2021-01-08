using Microservice.Badge.Infrastructure;
using Thunder.Platform.Cqrs;

namespace Microservice.Badge.Application.Commands
{
    public abstract class BaseCommandHandler<TCommand> : BaseThunderCommandHandler<TCommand> where TCommand : BaseThunderCommand
    {
        protected BaseCommandHandler(BadgeDbContext dbContext)
        {
            DbContext = dbContext;
        }

        protected BadgeDbContext DbContext { get; }
    }
}
