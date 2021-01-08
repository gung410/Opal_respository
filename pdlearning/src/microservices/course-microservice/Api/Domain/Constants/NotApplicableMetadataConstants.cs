using System.Collections.Generic;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Domain.Constants
{
    public static class NotApplicableMetadataConstants
    {
        public static readonly Dictionary<string, string[]> CourseNotApplicableMapping = new Dictionary<string, string[]>
        {
            [nameof(CourseEntity.ServiceSchemeIds)] = MetadataTagConstants.ServiceSchemeNotApplicableTagIds,
            [nameof(CourseEntity.TeachingLevels)] = MetadataTagConstants.TeachingLevelNotApplicableTagIds,
            [nameof(CourseEntity.CourseLevel)] = MetadataTagConstants.CourseLevelNotApplicableTagIds
        };

        public static readonly Dictionary<string, string[]> UserNotApplicableMapping = new Dictionary<string, string[]>
        {
            [nameof(CourseUser.ServiceScheme)] = MetadataTagConstants.ServiceSchemeNotApplicableTagIds,
            [nameof(CourseUser.TeachingSubject)] = MetadataTagConstants.TeachingSubjectNotApplicableTagIds,
            [nameof(CourseUser.TeachingLevel)] = MetadataTagConstants.TeachingLevelNotApplicableTagIds,
            [nameof(CourseUser.TeachingCourseOfStudy)] = MetadataTagConstants.TeachingCourseStudytNotApplicableTagIds,
            [nameof(CourseUser.CocurricularActivity)] = MetadataTagConstants.CocurricularActivitytNotApplicableTagIds
        };
    }
}
