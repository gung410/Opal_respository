using System;

namespace Microservice.Course.Domain.Constants
{
    public static class MetadataTagConstants
    {
        public static readonly Guid EasSubstantiveGradeBanding = new Guid("4a4bcf3a-9e31-11e9-9939-0242ac120003");

        public static readonly string MicroLearningTagId = "db13d0f8-d595-11e9-baec-0242ac120004";

        // Region Mode of Learning
        public static readonly string ELearningTagId = "5df4dfda-db9f-11e9-b8d9-0242ac120004";

        public static readonly string FaceToFaceTagId = "4fff9b22-db9f-11e9-900a-0242ac120004";

        public static readonly string BlendedLearningTagId = "645d6202-db9f-11e9-9101-0242ac120004";

        // End region

        // Region Not Applicable Tag
        public static readonly string[] ServiceSchemeNotApplicableTagIds = { "72a1df40-d592-11e9-9740-0242ac120004" };
        public static readonly string[] CourseLevelNotApplicableTagIds = { "673649d6-e09e-11e9-bb1a-0242ac120004" };
        public static readonly string[] TrackNotApplicableTagIds = { "cffcc134-473c-11ea-9e5b-0242ac120003" };
        public static readonly string[] TeachingSubjectNotApplicableTagIds = { "5bf1cfc2-5782-11ea-a6bf-0242ac120003" };
        public static readonly string[] TeachingLevelNotApplicableTagIds = { "73ecd6a4-2c61-11ea-8e4e-0242ac120003" };
        public static readonly string[] TeachingCourseStudytNotApplicableTagIds = { "b1843ab8-1b2a-11ea-94b5-0242ac120003", "8e3b916a-b436-11e9-991c-0242ac120004" };
        public static readonly string[] CocurricularActivitytNotApplicableTagIds = { "def3b6fc-0b64-11ea-8bb4-0242ac120003" };

        // End region
    }
}
