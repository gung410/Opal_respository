using System;

namespace Conexus.Opal.Connector.RabbitMQ.Contract
{
    /// <summary>
    /// This interface is for contracting purpose only.
    /// </summary>
    public interface IMQMessageHasCreatedDate
    {
        DateTime? MessageCreatedDate { get; }
    }
}
