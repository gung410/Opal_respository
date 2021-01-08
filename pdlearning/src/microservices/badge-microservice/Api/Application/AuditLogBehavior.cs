using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Humanizer;
using MediatR;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.Badge.Application
{
    public class AuditLogBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IOpalMessageProducer _producer;
        private readonly IUserContext _userContext;
        private readonly IAuditLogConventions _auditLogConventions;
        private readonly RabbitMQOptions _options;

        public AuditLogBehavior(
            IOpalMessageProducer producer,
            IUserContext userContext,
            IOptions<RabbitMQOptions> options,
            IAuditLogConventions auditLogConventions)
        {
            _producer = producer;
            _userContext = userContext;
            _auditLogConventions = auditLogConventions;
            _options = options.Value;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = await next();

            // Only send audit log event to Audit Log Exchange after the process completed.
            if (request is BaseThunderCommand)
            {
                var commandTypeName = request.GetType().Name;
                var actionType = _auditLogConventions.GetActionTypeBy(commandTypeName);
                var commandName = commandTypeName.Humanize();
                var auditLogEvent = new MQAuditMessage<TRequest>
                {
                    Name = commandName,
                    UserId = _userContext.GetValue<string>(CommonUserContextKeys.UserId),
                    OriginIp = _userContext.GetValue<string>(CommonUserContextKeys.OriginIp),
                    ModuleName = "Badge",
                    ServiceName = "badge-service",
                    Body = request,
                    ActionType = actionType
                };

                await _producer.SendAsync(auditLogEvent, _options.DefaultAuditLogExchange, MQAuditMessage<TRequest>.DefaultRoutingKey);
            }

            return response;
        }
    }
}
