using System;
using System.Collections.Generic;
using Microservice.Course.Application.Commands;

namespace Microservice.Course.Application.RequestDtos
{
    public class SendCourseNominationAnnoucementRequest
    {
        public IEnumerable<int> Organisations { get; set; }

        public Guid CourseId { get; set; }

        public string Base64Message { get; set; }

        public bool SpecificOrganisation { get; set; }

        public string UserNameTag { get; set; }

        public string CourseTitleTag { get; set; }

        public SendCourseNominationAnnouncementCommand ToCommand()
        {
            return new SendCourseNominationAnnouncementCommand()
            {
                Organisations = Organisations,
                SpecificOrganisation = SpecificOrganisation,
                Base64Message = Base64Message,
                CourseId = CourseId,
                UserNameTag = UserNameTag,
                CourseTitleTag = CourseTitleTag,
            };
        }
    }
}
