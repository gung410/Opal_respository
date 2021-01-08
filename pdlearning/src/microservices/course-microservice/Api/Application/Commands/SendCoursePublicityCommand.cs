using System;
using System.Collections.Generic;
using System.Text;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class SendCoursePublicityCommand : BaseThunderCommand
    {
        public IEnumerable<Guid> UserIds { get; set; }

        public IEnumerable<string> TeachingSubjectIds { get; set; }

        public IEnumerable<string> TeachingLevels { get; set; }

        public Guid CourseId { get; set; }

        public string Base64Message { get; set; }

        public bool SpecificTargetAudience { get; set; }

        public string UserNameTag { get; set; }

        public string CourseTitleTag { get; set; }

        public string GetReplacedTagsMessage(string courseTitle, string userName)
        {
            var message = GetDecodedMessage();
            if (!string.IsNullOrEmpty(CourseTitleTag))
            {
                message = message.Replace(CourseTitleTag, courseTitle);
            }

            if (!string.IsNullOrEmpty(UserNameTag))
            {
                message = message.Replace(UserNameTag, userName);
            }

            return message;
        }

        public string GetDecodedMessage()
        {
            return Base64Message == null ? string.Empty : Encoding.UTF8.GetString(Convert.FromBase64String(Base64Message));
        }
    }
}
