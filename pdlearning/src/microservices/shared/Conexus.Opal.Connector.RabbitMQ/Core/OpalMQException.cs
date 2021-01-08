using Thunder.Platform.Core.Exceptions;

namespace Conexus.Opal.Connector.RabbitMQ.Core
{
    public class OpalMQException : GeneralException
    {
        public OpalMQException()
        {
        }

        public OpalMQException(string message)
            : base(message)
        {
        }
    }
}
