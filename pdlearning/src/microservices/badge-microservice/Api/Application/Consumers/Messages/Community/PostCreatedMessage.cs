using System;
using System.Text.RegularExpressions;
using Microservice.Badge.Application.Consumers.Dtos;
using Microservice.Badge.Application.Enums;

namespace Microservice.Badge.Application.Consumers.Messages
{
    public class PostCreatedMessage
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public SourceType SourceType { get; set; }

        public SourceDto Source { get; set; }

        public bool HasContentForward { get; set; }

        public SourceType? ContentForwardType { get; set; }

        public PostDto ContentForward { get; set; }

        public bool HasLink()
        {
            var pattern = @"<a(?:\s+|\s.+\s)href=""(?<url>.+?)"".*\</a>";
            var result = Regex.IsMatch(this.Message, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            return result;
        }

        public int NumOfMultimedia()
        {
            string pattern = @"<figure";
            var regex = Regex.Matches(Message, pattern,  RegexOptions.Multiline);
            return regex.Count;
        }
    }
}
