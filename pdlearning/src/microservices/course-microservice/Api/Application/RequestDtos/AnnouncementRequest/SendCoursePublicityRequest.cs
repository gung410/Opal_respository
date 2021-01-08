using System;
using System.Collections.Generic;
using Microservice.Course.Application.Commands;

namespace Microservice.Course.Application.RequestDtos
{
    public class SendCoursePublicityRequest
    {
        public IEnumerable<Guid> UserIds { get; set; }

        public IEnumerable<string> TeachingSubjectIds { get; set; }

        public IEnumerable<string> TeachingLevels { get; set; }

        public Guid CourseId { get; set; }

        public string Base64Message { get; set; }

        public bool SpecificTargetAudience { get; set; }

        public string UserNameTag { get; set; }

        public string CourseTitleTag { get; set; }

        public SendCoursePublicityCommand ToCommand()
        {
            return new SendCoursePublicityCommand()
            {
                UserIds = UserIds,
                TeachingSubjectIds = TeachingSubjectIds,
                TeachingLevels = TeachingLevels,
                SpecificTargetAudience = SpecificTargetAudience,
                Base64Message = Base64Message,
                CourseId = CourseId,
                UserNameTag = UserNameTag,
                CourseTitleTag = CourseTitleTag,
            };
        }
    }
}
