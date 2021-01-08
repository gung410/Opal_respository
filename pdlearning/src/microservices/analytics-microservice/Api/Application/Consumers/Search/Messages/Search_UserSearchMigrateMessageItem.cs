using System;

namespace Microservice.Analytics.Application.Consumers.Search.Messages
{
    public class Search_UserSearchMigrateMessageItem
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Keyword { get; set; }

        public DateTime CreatedUtc { get; set; }
    }
}
