using System;
using System.Collections.Generic;
using System.Text;
using Microservice.Course.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class SendPlacementLetterCommand : BaseThunderCommand
    {
        public SendPlacementLetterCommand(SendPlacementLetterRequest request)
        {
            Ids = request.Ids;
            Base64Message = request.Base64Message;
            UserNameTag = request.UserNameTag;
            CourseTitleTag = request.CourseTitleTag;
            CourseCodeTag = request.CourseCodeTag;
            CourseAdminNameTag = request.CourseAdminNameTag;
            CourseAdminEmailTag = request.CourseAdminEmailTag;
            ListSessionTag = request.ListSessionTag;
            DetailUrlTag = request.DetailUrlTag;
        }

        public List<Guid> Ids { get; set; }

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
