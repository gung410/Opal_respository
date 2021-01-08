using Communication.Business.Models.Email;
using Communication.DataAccess;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Communication.Business.Models.Command
{

    public class CommunicationCommand
    {
        public CommunicationCommand()
        {
        }
        public string Type { get; set; }
        public string Version { get; set; }
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public Routing Routing { get; set; }
        public Payload Payload { get; set; }
    }

    public class Payload
    {
        public Identity Identity { get; set; }
        public References References { get; set; }
        public Body Body { get; set; }
    }

    public class Body
    {
        public bool DirectMessage { get; set; }
        public Recipient Recipient { get; set; }
        public Channel? Channel { get; set; }
        public Message Message { get; set; }
        public TemplateData TemplateData { get; set; }
    }

    public class Message
    {

        public string Subject { get; set; }
        public string PlainMessage { get; set; }
        public string PlainFormat { get; set; }
        public string DisplayMessage { get; set; }
        public string DisplayFormat { get; set; }
        public string LangCode { get; set; }
        public string Encoding { get; set; }
        public string CallbackCommand { get; set; }
        public string ExternalId { get; set; }
        public dynamic Data { get; set; }
        public List<Attachment> Attachments { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public MessageType? MessageType { get; set; }
        public string ClientId { get; set; }
        public bool? IsGlobal { get; set; }

    }

    public enum Channel
    {
        Default,
        Email,
        SMS,
        System,
        Banner,
    }

    public class Recipient
    {
        public HashSet<string> UserIds { get; set; }
        public bool? ForHrmsUsers { get; set; }
        public bool? ForExternalUsers { get; set; }
        public HashSet<string> Emails { get; set; }
        public HashSet<string> PhoneNumbers { get; set; }
        public HashSet<string> DepartmentIds { get; set; }
        public HashSet<string> RoleIds { get; set; }
        public HashSet<string> DepartmentTypeIds { get; set; }
        public HashSet<string> UserTypeIds { get; set; }
        public HashSet<string> UserGroupIds { get; set; }
        public HashSet<string> ClientIds { get; set; }
    }

    public class Identity
    {
        public string ClientId { get; set; }
        public string CustomerId { get; set; }
        public string UserId { get; set; }
    }

    public class References
    {
        public string CorrelationId { get; set; }
        public string SourceIp { get; set; }
    }

    public class Routing
    {
        public string Action { get; set; }
        public string ActionVersion { get; set; }
        public string Entity { get; set; }
        public string EntityId { get; set; }
    }

}
