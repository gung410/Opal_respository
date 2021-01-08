using System;
using System.Text;
using Microservice.Course.Application.Models;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class PreviewAnnouncementTemplateQuery : BaseThunderQuery<PreviewAnnouncementTemplateModel>
    {
        public AnnouncementType AnnouncementType { get; set; }

        public Guid ClassRunId { get; set; }

        public string Base64Message { get; set; }

        public string UserNameTag { get; set; }

        public string CourseTitleTag { get; set; }

        public string CourseCodeTag { get; set; }

        public string CourseAdminNameTag { get; set; }

        public string CourseAdminEmailTag { get; set; }

        public string ListSessionTag { get; set; }

        public string DetailUrlTag { get; set; }

        public string GetReplacedTagsMessage(string courseTitle, string userName, string courseCode, string courseAdminName, string courseAdminEmail, string listSession, string detailUrlTag)
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

            if (!string.IsNullOrEmpty(CourseCodeTag))
            {
                message = message.Replace(CourseCodeTag, courseCode);
            }

            if (!string.IsNullOrEmpty(CourseAdminNameTag))
            {
                message = message.Replace(CourseAdminNameTag, courseAdminName);
            }

            if (!string.IsNullOrEmpty(CourseAdminEmailTag))
            {
                message = message.Replace(CourseAdminEmailTag, courseAdminEmail);
            }

            if (!string.IsNullOrEmpty(DetailUrlTag))
            {
                message = message.Replace(DetailUrlTag, $"<a clicktracking=off href=\"{detailUrlTag}\" style=\"color:blue\" target=\"_blank\">{detailUrlTag}</a>");
            }

            if (!string.IsNullOrEmpty(ListSessionTag))
            {
                message = message.Replace(ListSessionTag, listSession);
            }

            return message;
        }

        public string GetDecodedMessage()
        {
            return Base64Message == null ? string.Empty : Encoding.UTF8.GetString(Convert.FromBase64String(Base64Message));
        }
    }
}
