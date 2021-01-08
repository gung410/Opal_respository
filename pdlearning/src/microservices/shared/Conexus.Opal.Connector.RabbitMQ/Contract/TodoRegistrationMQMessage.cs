using System;
using System.Collections.Generic;

#pragma warning disable SA1402 // File may only contain a single type
namespace Conexus.Opal.Connector.RabbitMQ.Contract
{
    public enum ReminderByType
    {
        /// <summary>
        /// To run a reminder tasks at an exact datetime.
        /// </summary>
        AbsoluteDateTimeUTC,

        /// <summary>
        /// To run a reminder tasks based on start date to calculate time execution.
        /// </summary>
        WorkdaysBeforeStartDate,

        /// <summary>
        /// To run a reminder tasks based on start date to calculate time execution.
        /// </summary>
        WorkdaysAfterStartDate,

        /// <summary>
        /// To run a reminder tasks run the reminder before the deadline.
        /// </summary>
        WorkdaysBeforeDeadline
    }

    public enum ToDoType
    {
        Notification
    }

    /// <summary>
    /// The contract was described at https://cxtech.atlassian.net/wiki/spaces/MP/pages/1120469047/To-Do+API+Processor+with+reminder+and+escalation+handling.
    /// The reference contract from the consumer: https://bitbucket.org/cxdev/cx-todo-api.
    /// ROUTING KEY: "todo.register".
    /// </summary>
    public class TodoRegistrationMQMessage : IMQMessage
    {
        public const string RoutingKey = "todo.register";

        /// <summary>
        /// The unique ID for the task.
        /// <example>
        /// The convention could be like this: "pdpm/lna/xxx-xxxxx-xxxxx-xxxx"
        /// </example>
        /// </summary>
        public string TaskURI { get; set; }

        /// <summary>
        /// TODO: why this is optional? Need to document more clear.
        /// </summary>
        public DateTime? StartDateUTC { get; set; }

        /// <summary>
        /// A primary person in charge for a task.
        /// </summary>
        public PersonInCharge Primary { get; set; }

        /// <summary>
        /// A secondary person in charge for a task.
        /// </summary>
        public PersonInCharge Secondary { get; set; }

        /// <summary>
        ///  Define this task in module?.
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// The notification template (by email, push notification, etc.) in Communication API,
        /// or use custom subject and message (other properties).
        /// <example>
        /// Template = "LearnerInWaitListClassRun"
        /// </example>
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// A set of custom replacement codes for template or message.
        /// </summary>
        public IDictionary<string, object> TemplateData { get; set; }

        /// <summary>
        /// TODO: Source code from https://bitbucket.org/cxdev/cx-todo-api contains this fields.
        /// But the contract from the Confluence does not!.
        /// </summary>
        public string CreatedBy { get; set; }
    }

    public class PersonInCharge
    {
        public List<ReceiverDto> AssignedTo { get; set; }

        public List<ReminderByDto> ReminderBy { get; set; }

        /// <summary>
        /// Optional.
        /// TODO: Could be the deadline of approval or ?.
        /// </summary>
        public DateTime? DeadlineUTC { get; set; }

        /// <summary>
        /// No needed if template is set.
        /// <example>
        /// Subject = "Approval of {{courseTitle}}"
        /// </example>
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// No needed if template is set.
        /// <example>
        /// Subject = "Dear {{givenName}} this is an example"
        /// </example>
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// for InApp (No needed if template is set).
        /// <example>
        /// Subject = "Dear {{givenName}} this is an example"
        /// </example>
        /// </summary>
        public string PlainText { get; set; }
    }

    public class ReceiverDto
    {
        public string UserId { get; set; }

        public string DirectEmail { get; set; }

        // This is optional, we don't need it, just skip it!
        // public Dictionary<string, object> ReferenceData { get; set; }
    }

    public class ReminderByDto
    {
        /// <summary>
        /// The reminder type <see cref="ReminderByType"/>.
        /// </summary>
        public ReminderByType Type { get; set; }

        /// <summary>
        /// The setting of the reminder task.
        /// <example>
        ///     {
        ///       "type": "AbsoluteDateTimeUTC", // run reminder at exact datetime
        ///       "value": "2020-04-04T09:20:10.587Z"
        ///     },
        ///     {
        ///       "type": "WorkdaysBeforeStartDate", // based on start date to calculate time execution
        ///       "value": "5" // put integer number of days as string format
        ///     },
        ///     {
        ///       "type": "WorkdaysAfterStartDate", // based on start date to calculate time execution
        ///       "value": "5" // put integer number of days as string format
        ///     },
        ///     {
        ///       "type": "WorkdaysBeforeDeadline", // run the reminder before the dealine
        ///       "value": "2" // put integer number of days as string format
        ///     }
        /// </example>
        /// </summary>
        public string Value { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
