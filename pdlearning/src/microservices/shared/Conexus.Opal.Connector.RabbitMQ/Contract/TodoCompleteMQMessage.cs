#pragma warning disable SA1402 // File may only contain a single type
namespace Conexus.Opal.Connector.RabbitMQ.Contract
{
    public enum Status
    {
        Complete,
        Cancel
    }

    /// <summary>
    /// The contract was described at https://cxtech.atlassian.net/wiki/spaces/MP/pages/1120469047/To-Do+API+Processor+with+reminder+and+escalation+handling.
    /// The reference contract from the consumer: https://bitbucket.org/cxdev/cx-todo-api.
    /// ROUTING KEY: "todo.complete".
    /// </summary>
    public class TodoCompleteMQMessage : IMQMessage
    {
        public const string RoutingKey = "todo.complete";

        /// <summary>
        /// The unique ID for the task.
        /// <example>
        /// The convention could be like this: "pdpm/lna/xxx-xxxxx-xxxxx-xxxx"
        /// </example>
        /// </summary>
        public string TaskURI { get; set; }

        public Status Status { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
