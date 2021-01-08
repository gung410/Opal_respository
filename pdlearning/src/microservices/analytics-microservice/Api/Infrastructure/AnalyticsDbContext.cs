using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure
{
    public class AnalyticsDbContext : BaseThunderDbContext
    {
        private readonly IConnectionStringResolver _connectionStringResolver;

        public AnalyticsDbContext(IConnectionStringResolver connectionStringResolver)
        {
            _connectionStringResolver = connectionStringResolver;
        }

        public virtual DbSet<CAM_Assignment> CamAssignment { get; set; }

        public virtual DbSet<CAM_AttendanceTracking> CamAttendanceTracking { get; set; }

        public virtual DbSet<CAM_ClassRun> CamClassRun { get; set; }

        public virtual DbSet<CAM_ClassRunFacilitator> CamClassRunFacilitator { get; set; }

        public virtual DbSet<CAM_Course> CamCourse { get; set; }

        public virtual DbSet<CAM_CourseApplicableBranch> CamCourseApplicableBranch { get; set; }

        public virtual DbSet<CAM_CourseApplicableCluster> CamCourseApplicableCluster { get; set; }

        public virtual DbSet<CAM_CourseApplicableDivision> CamCourseApplicableDivision { get; set; }

        public virtual DbSet<CAM_CourseApplicableSchool> CamCourseApplicableSchool { get; set; }

        public virtual DbSet<CAM_CourseApplicableZone> CamCourseApplicableZone { get; set; }

        public virtual DbSet<CAM_CourseCategory> CamCourseCategory { get; set; }

        public virtual DbSet<CAM_CourseCoCurricularActivity> CamCourseCoCurricularActivity { get; set; }

        public virtual DbSet<CAM_CourseCoFacilitator> CamCourseCoFacilitator { get; set; }

        public virtual DbSet<CAM_CourseCollaborativeContentCreator> CamCourseCollaborativeContentCreator { get; set; }

        public virtual DbSet<CAM_CourseCourseOfStudy> CamCourseCourseOfStudy { get; set; }

        public virtual DbSet<CAM_CourseDevelopmentRole> CamCourseDevelopmentRole { get; set; }

        public virtual DbSet<CAM_CourseEasSubstantiveGradeBanding> CamCourseEasSubstantiveGradeBanding { get; set; }

        public virtual DbSet<CAM_CourseFacilitator> CamCourseFacilitator { get; set; }

        public virtual DbSet<CAM_CourseHistory> CamCourseHistory { get; set; }

        public virtual DbSet<CAM_CourseJobFamily> CamCourseJobFamily { get; set; }

        public virtual DbSet<CAM_CourseLearningArea> CamCourseLearningArea { get; set; }

        public virtual DbSet<CAM_CourseLearningDimension> CamCourseLearningDimension { get; set; }

        public virtual DbSet<CAM_CourseLearningFramework> CamCourseLearningFramework { get; set; }

        public virtual DbSet<CAM_CourseLearningSubArea> CamCourseLearningSubArea { get; set; }

        public virtual DbSet<CAM_CourseNieAcademicGroups> CamCourseNieAcademicGroups { get; set; }

        public virtual DbSet<CAM_CourseOtherTrainingAgencyReason> CamCourseOtherTrainingAgencyReason { get; set; }

        public virtual DbSet<CAM_CourseOwnerBranch> CamCourseOwnerBranch { get; set; }

        public virtual DbSet<CAM_CourseOwnerDivision> CamCourseOwnerDivision { get; set; }

        public virtual DbSet<CAM_CoursePdperiod> CamCoursePdperiod { get; set; }

        public virtual DbSet<CAM_CoursePlanningCycle> CamCoursePlanningCycle { get; set; }

        public virtual DbSet<CAM_CoursePreRequisite> CamCoursePreRequisite { get; set; }

        public virtual DbSet<CAM_CourseServiceScheme> CamCourseServiceScheme { get; set; }

        public virtual DbSet<CAM_CourseTeachingLevel> CamCourseTeachingLevel { get; set; }

        public virtual DbSet<CAM_CourseTeachingSubject> CamCourseTeachingSubject { get; set; }

        public virtual DbSet<CAM_EcertificateTemplate> CamEcertificateTemplate { get; set; }

        public virtual DbSet<CAM_Lecture> CamLecture { get; set; }

        public virtual DbSet<CAM_LectureContent> CamLectureContent { get; set; }

        public virtual DbSet<CAM_ParticipantAssignmentTrack> CamParticipantAssignmentTrack { get; set; }

        public virtual DbSet<CAM_ParticipantAssignmentTrackQuizAnswer> CamParticipantAssignmentTrackQuizAnswer { get; set; }

        public virtual DbSet<CAM_ParticipantAssignmentTrackQuizQuestionAnswer> CamParticipantAssignmentTrackQuizQuestionAnswer { get; set; }

        public virtual DbSet<CAM_QuizAssignmentForm> CamQuizAssignmentForm { get; set; }

        public virtual DbSet<CAM_QuizAssignmentFormQuestion> CamQuizAssignmentFormQuestion { get; set; }

        public virtual DbSet<CAM_Registration> CamRegistration { get; set; }

        public virtual DbSet<CAM_Section> CamSection { get; set; }

        public virtual DbSet<CAM_Session> CamSession { get; set; }

        public virtual DbSet<CCPM_DigitalContent> CcpmDigitalContent { get; set; }

        public virtual DbSet<CCPM_DigitalContentComments> CcpmDigitalContentComments { get; set; }

        public virtual DbSet<CCPM_Form> CcpmForm { get; set; }

        public virtual DbSet<CCPM_FormAnswer> CcpmFormAnswer { get; set; }

        public virtual DbSet<CCPM_FormComment> CcpmFormComment { get; set; }

        public virtual DbSet<CCPM_FormParticipant> CcpmFormParticipant { get; set; }

        public virtual DbSet<CCPM_FormQuestion> CcpmFormQuestion { get; set; }

        public virtual DbSet<CCPM_FormQuestionAnswer> CcpmFormQuestionAnswer { get; set; }

        public virtual DbSet<CCPM_DigitalContentTracking> CcpmDigitalContentTracking { get; set; }

        public virtual DbSet<CSL_CommentForumThread> CslCommentForumThread { get; set; }

        public virtual DbSet<CSL_CommentPoll> CslCommentPoll { get; set; }

        public virtual DbSet<CSL_CommentPost> CslCommentPost { get; set; }

        public virtual DbSet<CSL_CommentWikiPage> CslCommentWikiPage { get; set; }

        public virtual DbSet<CSL_FileComment> CslFileComment { get; set; }

        public virtual DbSet<CSL_FileForumThread> CslFileForumThread { get; set; }

        public virtual DbSet<CSL_FilePoll> CslFilePoll { get; set; }

        public virtual DbSet<CSL_FilePost> CslFilePost { get; set; }

        public virtual DbSet<CSL_FileWikiPage> CslFileWikiPage { get; set; }

        public virtual DbSet<CSL_ForumThread> CslForumThread { get; set; }

        public virtual DbSet<CSL_ForwardForumThread> CslForwardForumThread { get; set; }

        public virtual DbSet<CSL_ForwardPost> CslForwardPost { get; set; }

        public virtual DbSet<CSL_ForwardWikiPage> CslForwardWikiPage { get; set; }

        public virtual DbSet<CSL_Group> CslGroup { get; set; }

        public virtual DbSet<CSL_GroupPermission> CslGroupPermission { get; set; }

        public virtual DbSet<CSL_GroupUser> CslGroupUser { get; set; }

        public virtual DbSet<CSL_LikeComment> CslLikeComment { get; set; }

        public virtual DbSet<CSL_LikeForumThread> CslLikeForumThread { get; set; }

        public virtual DbSet<CSL_LikePoll> CslLikePoll { get; set; }

        public virtual DbSet<CSL_LikePost> CslLikePost { get; set; }

        public virtual DbSet<CSL_LikeWikiPage> CslLikeWikiPage { get; set; }

        public virtual DbSet<CSL_Poll> CslPoll { get; set; }

        public virtual DbSet<CSL_PollAnswerUser> CslPollAnswerUser { get; set; }

        public virtual DbSet<CSL_PollOptions> CslPollOptions { get; set; }

        public virtual DbSet<CSL_Post> CslPost { get; set; }

        public virtual DbSet<CSL_Space> CslSpace { get; set; }

        public virtual DbSet<CSL_SpaceMembership> CslSpaceMembership { get; set; }

        public virtual DbSet<CSL_User> CslUser { get; set; }

        public virtual DbSet<CSL_UserFollowForumThread> CslUserFollowForumThread { get; set; }

        public virtual DbSet<CSL_UserFollowPoll> CslUserFollowPoll { get; set; }

        public virtual DbSet<CSL_UserFollowPost> CslUserFollowPost { get; set; }

        public virtual DbSet<CSL_UserFollowSpace> CslUserFollowSpace { get; set; }

        public virtual DbSet<CSL_UserFollowUser> CslUserFollowUser { get; set; }

        public virtual DbSet<CSL_UserFollowWikiPage> CslUserFollowWikiPage { get; set; }

        public virtual DbSet<CSL_WikiPage> CslWikiPage { get; set; }

        public virtual DbSet<CSL_WikiPageRevision> CslWikiPageRevision { get; set; }

        public virtual DbSet<CSL_ForumThreadRevision> CslForumThreadRevision { get; set; }

        public virtual DbSet<MT_Category> MtCategory { get; set; }

        public virtual DbSet<MT_CoCurricularActivity> MtCoCurricularActivity { get; set; }

        public virtual DbSet<MT_CourseLevel> MtCourseLevel { get; set; }

        public virtual DbSet<MT_CourseOfStudy> MtCourseOfStudy { get; set; }

        public virtual DbSet<MT_DevelopmentRole> MtDevelopmentRole { get; set; }

        public virtual DbSet<MT_JobFamily> MtJobFamily { get; set; }

        public virtual DbSet<MT_LearningArea> MtLearningArea { get; set; }

        public virtual DbSet<MT_LearningDimension> MtLearningDimension { get; set; }

        public virtual DbSet<MT_LearningFramework> MtLearningFramework { get; set; }

        public virtual DbSet<MT_LearningMode> MtLearningMode { get; set; }

        public virtual DbSet<MT_LearningSubArea> MtLearningSubArea { get; set; }

        public virtual DbSet<MT_NatureOfCourse> MtNatureOfCourse { get; set; }

        public virtual DbSet<MT_PdactivityType> MtPdactivityType { get; set; }

        public virtual DbSet<MT_Pdperiod> MtPdperiod { get; set; }

        public virtual DbSet<MT_ServiceScheme> MtServiceScheme { get; set; }

        public virtual DbSet<MT_Subject> MtSubject { get; set; }

        public virtual DbSet<MT_SubjectGroup> MtSubjectGroup { get; set; }

        public virtual DbSet<MT_SubjectKeyWords> MtSubjectKeyWords { get; set; }

        public virtual DbSet<MT_TeachingLevel> MtTeachingLevel { get; set; }

        public virtual DbSet<MT_TeachingSubject> MtTeachingSubject { get; set; }

        public virtual DbSet<MT_Track> MtTrack { get; set; }

        public virtual DbSet<SAM_Archetypes> SamArchetypes { get; set; }

        public virtual DbSet<SAM_DepartmentDepartmentType> SamDepartmentDepartmentType { get; set; }

        public virtual DbSet<SAM_DepartmentTypes> SamDepartmentTypes { get; set; }

        public virtual DbSet<SAM_Department> SamDepartments { get; set; }

        public virtual DbSet<SAM_EntityStatusReasons> SamEntityStatusReasons { get; set; }

        public virtual DbSet<SAM_EntityStatuses> SamEntityStatuses { get; set; }

        public virtual DbSet<SAM_MemberRole> SamMemberRole { get; set; }

        public virtual DbSet<SAM_Ugmember> SamUgmember { get; set; }

        public virtual DbSet<SAM_User> SamUser { get; set; }

        public virtual DbSet<SAM_UserCoCurricularActivity> SamUserCoCurricularActivity { get; set; }

        public virtual DbSet<SAM_UserCourseOfStudy> SamUserCourseOfStudy { get; set; }

        public virtual DbSet<SAM_UserGroup> SamUserGroup { get; set; }

        public virtual DbSet<SAM_UserGroupType> SamUserGroupType { get; set; }

        public virtual DbSet<SAM_UserHistory> SamUserHistory { get; set; }

        public virtual DbSet<SAM_UserJobFamily> SamUserJobFamily { get; set; }

        public virtual DbSet<SAM_UserProfessionalInterestArea> SamUserProfessionalInterestArea { get; set; }

        public virtual DbSet<SAM_UserTeachingLevel> SamUserTeachingLevel { get; set; }

        public virtual DbSet<SAM_UserTeachingSubject> SamUserTeachingSubject { get; set; }

        public virtual DbSet<SAM_UserTrack> SamUserTrack { get; set; }

        public virtual DbSet<SAM_UserTypes> SamUserTypes { get; set; }

        public virtual DbSet<SAM_UserUserTypes> SamUserUserTypes { get; set; }

        public virtual DbSet<SAM_UserLogin> SamUserLogins { get; set; }

        public virtual DbSet<SAM_UserOtpClaim> SamUserOtpClaims { get; set; }

        public virtual DbSet<Learner_LecturesInUserCourse> LearnerLecturesInUserCourse { get; set; }

        public virtual DbSet<Learner_UserAssignment> LearnerUserAssignments { get; set; }

        public virtual DbSet<Learner_UserBookmarksCourse> LearnerUserBookmarksCourse { get; set; }

        public virtual DbSet<Learner_UserBookmarksDigitalContent> LearnerUserBookmarksDigitalContent { get; set; }

        public virtual DbSet<Learner_UserClassRun> LearnerUserClassRun { get; set; }

        public virtual DbSet<Learner_UserCourse> LearnerUserCourses { get; set; }

        public virtual DbSet<Learner_UserDigitalContent> LearnerUserDigitalContent { get; set; }

        public virtual DbSet<Learner_UserLearningPackage> LearnerUserLearningPackages { get; set; }

        public virtual DbSet<Learner_UserLearningPathCourse> LearnerUserLearningPathCourses { get; set; }

        public virtual DbSet<Learner_UserLearningPath> LearnerUserLearningPaths { get; set; }

        public virtual DbSet<Learner_UserLectureInCourse> LearnerUserLectureInCourse { get; set; }

        public virtual DbSet<Learner_UserReview> LearnerUserReviews { get; set; }

        public virtual DbSet<Learner_UserBookmarksSpace> LearnerUserBookmarksSpace { get; set; }

        public virtual DbSet<Learner_UserBookmarksLearnerLearningPath> LearnerUserBookmarksLearnerLearningPath { get; set; }

        public virtual DbSet<Learner_UserBookmarksLearningPath> LearnerUserBookmarksLearningPath { get; set; }

        public virtual DbSet<Learner_UserActivityAssignment> LearnerUserActivityAssignment { get; set; }

        public virtual DbSet<Learner_UserActivityLandingPage> LearnerUserActivityLandingPage { get; set; }

        public virtual DbSet<Learner_UserActivityLecture> LearnerUserActivityLecture { get; set; }

        public virtual DbSet<Learner_UserActivityPdoPage> LearnerUserActivityPdopage { get; set; }

        public virtual DbSet<Learner_UserActivityQuiz> LearnerUserActivityQuiz { get; set; }

        public virtual DbSet<Learner_UserActivityRevisitMlu> LearnerUserActivityRevisitMlu { get; set; }

        public virtual DbSet<Learner_UserActivityResume> LearnerUserActivityResume { get; set; }

        public virtual DbSet<SearchEngine> SearchEngine { get; set; }

        public virtual DbSet<SearchEngineCategory> SearchEngineCategory { get; set; }

        public virtual DbSet<SearchEngineDevelopmentRole> SearchEngineDevelopmentRole { get; set; }

        public virtual DbSet<SearchEngineLearningArea> SearchEngineLearningArea { get; set; }

        public virtual DbSet<SearchEngineLearningDimension> SearchEngineLearningDimension { get; set; }

        public virtual DbSet<SearchEngineLearningFramework> SearchEngineLearningFramework { get; set; }

        public virtual DbSet<SearchEngineLearningSubArea> SearchEngineLearningSubArea { get; set; }

        public virtual DbSet<SearchEngineServiceScheme> SearchEngineServiceScheme { get; set; }

        public virtual DbSet<SearchEngineSubject> SearchEngineSubject { get; set; }

        public virtual DbSet<SearchEngineTeachingLevel> SearchEngineTeachingLevel { get; set; }

        public virtual DbSet<PDPM_IDP_LNA_StatusHistory> PdpmIdpLnaStatusHistory { get; set; }

        public virtual DbSet<PDPM_IDP_PDO_StatusHistory> PdpmIdpPdoStatusHistory { get; set; }

        public virtual DbSet<PDPM_ODP_KLP_StatusHistory> PdpmOdpKlpStatusHistory { get; set; }

        public virtual DbSet<PDPM_ODP_LearningDirectionStatusHistory> PdpmOdpLearningDirectionStatusHistory { get; set; }

        public virtual DbSet<PDPM_ODP_LearningPlanStatusHistory> PdpmOdpLearningPlanStatusHistory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (DbConnection != null)
            {
                optionsBuilder.UseSqlServer(DbConnection);
            }
            else
            {
                optionsBuilder.UseSqlServer(ChooseConnectionString());
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            base.OnModelCreating(modelBuilder);
        }

        private string ChooseConnectionString()
        {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            // We want to explicit code here so please do not use ? :
            if (string.IsNullOrEmpty(ConnectionString))
            {
                return _connectionStringResolver.GetNameOrConnectionString(new ConnectionStringResolveArgs());
            }

            return ConnectionString;
        }
    }
}
