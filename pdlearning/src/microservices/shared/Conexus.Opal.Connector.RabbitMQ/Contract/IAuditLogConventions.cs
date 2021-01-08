using JetBrains.Annotations;

namespace Conexus.Opal.Connector.RabbitMQ.Contract
{
    public interface IAuditLogConventions
    {
        AuditLogActionType GetActionTypeBy([NotNull] string commandName);
    }
}
