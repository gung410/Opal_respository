using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Conexus.Opal.Connector.RabbitMQ.Contract
{
    public class AuditLogConventions : IAuditLogConventions
    {
        private readonly IReadOnlyDictionary<AuditLogActionType, string[]> _conventions;

        public AuditLogConventions(IReadOnlyDictionary<AuditLogActionType, string[]> conventions)
        {
            _conventions = conventions;
        }

        public AuditLogActionType GetActionTypeBy([NotNull] string commandName)
        {
            if (string.IsNullOrEmpty(commandName))
            {
                throw new ArgumentNullException(nameof(commandName));
            }

            var actionTypeQuery =
                from actionType in _conventions.Keys
                let conventions = _conventions[actionType]
                where conventions.Any(commandName.StartsWith)
                select actionType;

            return actionTypeQuery.FirstOrDefault();
        }
    }
}
