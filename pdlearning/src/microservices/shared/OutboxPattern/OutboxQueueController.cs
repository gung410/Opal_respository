using System.Threading.Tasks;
using Conexus.Opal.OutboxPattern;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Exceptions;
using Thunder.Service.Authentication;

namespace Conexus.Opal.Shared.OutboxPattern
{
    [Route("api/outbox-queue")]
    public class OutboxQueueController : ApplicationApiController
    {
        private readonly IOutboxQueue _outboxQueue;

        public OutboxQueueController(IOutboxQueue outboxQueue, IUserContext userContext) : base(userContext)
        {
            _outboxQueue = outboxQueue;
        }

        [HttpPost("requeue-message")]
        public async Task RequeueMessageAsync()
        {
            if (!UserContext.IsAdministrator())
            {
                throw new BusinessLogicException("You do not have permission to access this endpoint");
            }

            await _outboxQueue.RequeueMessageAsync();
        }
    }
}
