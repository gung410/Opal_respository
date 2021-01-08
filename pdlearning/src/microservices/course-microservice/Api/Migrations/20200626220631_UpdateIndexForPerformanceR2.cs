using Microservice.Course.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateIndexForPerformanceR2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE dbo.Assignment SET ChangedDate = CreatedDate Where ChangedDate IS NULL");
            migrationBuilder.Sql("UPDATE dbo.AttendanceTracking SET ChangedDate = CreatedDate Where ChangedDate IS NULL");
            migrationBuilder.Sql("UPDATE dbo.ClassRun SET ChangedDate = CreatedDate Where ChangedDate IS NULL");
            migrationBuilder.Sql("UPDATE dbo.Comment SET ChangedDate = CreatedDate Where ChangedDate IS NULL");
            migrationBuilder.Sql("UPDATE dbo.Course SET ChangedDate = CreatedDate Where ChangedDate IS NULL");
            migrationBuilder.Sql("UPDATE dbo.CoursePlanningCycle SET ChangedDate = CreatedDate Where ChangedDate IS NULL");
            migrationBuilder.Sql("UPDATE dbo.LearningPath SET ChangedDate = CreatedDate Where ChangedDate IS NULL");
            migrationBuilder.Sql("UPDATE dbo.Registration SET ChangedDate = CreatedDate Where ChangedDate IS NULL");
            migrationBuilder.Sql("UPDATE dbo.Course SET ChangedDate = CreatedDate Where ChangedDate IS NULL");

            RemoveIndexIfExist(migrationBuilder);

            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Session_IsDeleted_CreatedDate] ON [dbo].[Session]([IsDeleted] ASC,[CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Session_ClassRunId_IsDeleted_CreatedDate] ON [dbo].[Session]([ClassRunId] ASC,[IsDeleted] ASC,[CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Session_CreatedBy_IsDeleted_CreatedDate] ON [dbo].[Session]([CreatedBy] ASC,[IsDeleted] ASC,[CreatedDate] DESC)");

            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Section_IsDeleted_CreatedDate] ON [dbo].[Section]([IsDeleted] ASC,[CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Section_ClassRunId_IsDeleted_CreatedDate] ON [dbo].[Section]([ClassRunId] ASC,[IsDeleted] ASC,[CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Section_CourseId_IsDeleted_CreatedDate] ON [dbo].[Section]([CourseId] ASC,[IsDeleted] ASC,[CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Section_CreatedBy_IsDeleted_CreatedDate] ON [dbo].[Section]([CreatedBy] ASC,[IsDeleted] ASC,[CreatedDate] DESC)");

            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Registration_IsDeleted_CreatedDate] ON [dbo].[Registration] ([IsDeleted] ASC,[CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Registration_ClassRunChangeId_IsDeleted_CreatedDate] ON [dbo].[Registration] ([ClassRunChangeId] ASC, [IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Registration_ClassRunChangeStatus_IsDeleted_CreatedDate] ON [dbo].[Registration] ([ClassRunChangeStatus] ASC, [IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Registration_ClassRunId_IsDeleted_CreatedDate] ON [dbo].[Registration] ([ClassRunId] ASC, [IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Registration_CourseId_IsDeleted_CreatedDate] ON [dbo].[Registration] ([CourseId] ASC, [IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Registration_CreatedBy_IsDeleted_CreatedDate] ON [dbo].[Registration] ([CreatedBy] ASC, [IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Registration_Status_IsDeleted_CreatedDate] ON [dbo].[Registration] ([Status] ASC, [IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Registration_UserId_IsDeleted_CreatedDate] ON [dbo].[Registration] ([UserId] ASC, [IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Registration_WithdrawalStatus_IsDeleted_CreatedDate] ON [dbo].[Registration] ([WithdrawalStatus] ASC, [IsDeleted] ASC, [CreatedDate] DESC)");

            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_ParticipantAssignmentTrack_IsDeleted_AssignedDate] ON [dbo].[ParticipantAssignmentTrack] ([IsDeleted] ASC, [AssignedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_ParticipantAssignmentTrack_IsDeleted_CreatedDate] ON [dbo].[ParticipantAssignmentTrack] ([IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_ParticipantAssignmentTrack_IsDeleted_SubmittedDate] ON [dbo].[ParticipantAssignmentTrack] ([IsDeleted] ASC, [SubmittedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_ParticipantAssignmentTrack_AssignmentId_IsDeleted_CreatedDate] ON [dbo].[ParticipantAssignmentTrack] ([AssignmentId] ASC, [IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_ParticipantAssignmentTrack_CreatedBy_IsDeleted_CreatedDate] ON [dbo].[ParticipantAssignmentTrack] ([CreatedBy] ASC, [IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_ParticipantAssignmentTrack_RegistrationId_IsDeleted_CreatedDate] ON [dbo].[ParticipantAssignmentTrack] ([RegistrationId] ASC, [IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_ParticipantAssignmentTrack_UserId_IsDeleted_CreatedDate] ON [dbo].[ParticipantAssignmentTrack] ([UserId] ASC, [IsDeleted] ASC, [CreatedDate] DESC)");

            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_LectureContent_IsDeleted_CreatedDate] ON [dbo].[LectureContent] ([IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_LectureContent_CreatedBy_IsDeleted_CreatedDate] ON [dbo].[LectureContent] ([CreatedBy], [IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_LectureContent_LectureId_IsDeleted_CreatedDate] ON [dbo].[LectureContent] ([LectureId], [IsDeleted] ASC, [CreatedDate] DESC)");

            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Lecture_IsDeleted_CreatedDate] ON [dbo].[Lecture] ([IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Lecture_ClassRunId_IsDeleted_CreatedDate] ON [dbo].[Lecture] ([ClassRunId], [IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Lecture_CourseId_IsDeleted_CreatedDate] ON [dbo].[Lecture] ([CourseId], [IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Lecture_CreatedBy_IsDeleted_CreatedDate] ON [dbo].[Lecture] ([CreatedBy], [IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Lecture_SectionId_IsDeleted_CreatedDate] ON [dbo].[Lecture] ([SectionId], [IsDeleted] ASC, [CreatedDate] DESC)");

            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_LearningPathCourse_IsDeleted_CreatedDate] ON [dbo].[LearningPathCourse] ([IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_LearningPathCourse_CourseId_IsDeleted_CreatedDate] ON [dbo].[LearningPathCourse] ([CourseId], [IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_LearningPathCourse_LearningPathId_IsDeleted_CreatedDate] ON [dbo].[LearningPathCourse] ([LearningPathId], [IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_LearningPathCourse_Order_IsDeleted_CreatedDate] ON [dbo].[LearningPathCourse] ([Order], [IsDeleted] ASC, [CreatedDate] DESC)");

            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_LearningPath_IsDeleted_CreatedDate] ON [dbo].[LearningPath] ([IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_LearningPath_CreatedBy_IsDeleted_CreatedDate] ON [dbo].[LearningPath] ([CreatedBy], [IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_LearningPath_Status_IsDeleted_CreatedDate] ON [dbo].[LearningPath] ([Status], [IsDeleted] ASC, [CreatedDate] DESC)");

            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_CoursePlanningCycle_IsDeleted_CreatedDate] ON [dbo].[CoursePlanningCycle] ([IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_CoursePlanningCycle_YearCycle_IsDeleted_CreatedDate] ON [dbo].[CoursePlanningCycle] ([YearCycle] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");

            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Course_IsDeleted_CreatedDate] ON [dbo].[Course] ([IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Course_IsDeleted_ChangedDate] ON [dbo].[Course] ([IsDeleted] ASC, [ChangedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Course_IsDeleted_VerifiedDate] ON [dbo].[Course] ([IsDeleted] ASC, [VerifiedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Course_AlternativeApprovingOfficerId_IsDeleted_CreatedDate] ON [dbo].[Course] ([AlternativeApprovingOfficerId] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Course_ContentStatus_IsDeleted_CreatedDate] ON [dbo].[Course] ([ContentStatus] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Course_CourseCode_IsDeleted_CreatedDate] ON [dbo].[Course] ([CourseCode] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Course_ExternalCode_IsDeleted_CreatedDate] ON [dbo].[Course] ([ExternalCode] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Course_CreatedBy_IsDeleted_CreatedDate] ON [dbo].[Course] ([CreatedBy] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Course_DepartmentId_IsDeleted_CreatedDate] ON [dbo].[Course] ([DepartmentId] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Course_ECertificateTemplateId_IsDeleted_CreatedDate] ON [dbo].[Course] ([ECertificateTemplateId] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Course_FirstAdministratorId_IsDeleted_CreatedDate] ON [dbo].[Course] ([FirstAdministratorId] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Course_MOEOfficerId_IsDeleted_CreatedDate] ON [dbo].[Course] ([MOEOfficerId] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Course_PDAreaThemeId_IsDeleted_CreatedDate] ON [dbo].[Course] ([PDAreaThemeId] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Course_PrimaryApprovingOfficerId_IsDeleted_CreatedDate] ON [dbo].[Course] ([PrimaryApprovingOfficerId] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Course_SecondAdministratorId_IsDeleted_CreatedDate] ON [dbo].[Course] ([SecondAdministratorId] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Course_Status_IsDeleted_CreatedDate] ON [dbo].[Course] ([Status] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");

            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Comment_CreatedDate] ON [dbo].[Comment] ([CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Comment_Action_CreatedDate] ON [dbo].[Comment] ([Action] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Comment_ObjectId_CreatedDate] ON [dbo].[Comment] ([ObjectId] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Comment_UserId_CreatedDate] ON [dbo].[Comment] ([UserId] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Comment_UserId_ObjectId_CreatedDate] ON [dbo].[Comment] ([UserId] ASC, [ObjectId] ASC, [CreatedDate] DESC)");

            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_ClassRun_CancellationStatus_IsDeleted_CreatedDate] ON [dbo].[ClassRun] ([CancellationStatus] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_ClassRun_ClassRunCode_IsDeleted_CreatedDate] ON [dbo].[ClassRun] ([ClassRunCode] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_ClassRun_ClassRunVenueId_IsDeleted_CreatedDate] ON [dbo].[ClassRun] ([ClassRunVenueId] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_ClassRun_ContentStatus_IsDeleted_CreatedDate] ON [dbo].[ClassRun] ([ContentStatus] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_ClassRun_CourseId_IsDeleted_CreatedDate] ON [dbo].[ClassRun] ([CourseId] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_ClassRun_CreatedBy_IsDeleted_CreatedDate] ON [dbo].[ClassRun] ([CreatedBy] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_ClassRun_RescheduleStatus_IsDeleted_CreatedDate] ON [dbo].[ClassRun] ([RescheduleStatus] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_ClassRun_Status_IsDeleted_CreatedDate] ON [dbo].[ClassRun] ([Status] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");

            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_AttendanceTracking_IsDeleted_CreatedDate] ON [dbo].[AttendanceTracking] ([IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_AttendanceTracking_RegistrationId_IsDeleted_CreatedDate] ON [dbo].[AttendanceTracking] ([RegistrationId] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_AttendanceTracking_SessionId_IsDeleted_CreatedDate] ON [dbo].[AttendanceTracking] ([SessionId] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_AttendanceTracking_Status_IsDeleted_CreatedDate] ON [dbo].[AttendanceTracking] ([Status] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_AttendanceTracking_Userid_IsDeleted_CreatedDate] ON [dbo].[AttendanceTracking] ([Userid] ASC,[IsDeleted] ASC, [CreatedDate] DESC)");

            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Assignment_IsDeleted_CreatedDate] ON [dbo].[Assignment] ([IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Assignment_ClassRunId_IsDeleted_CreatedDate] ON [dbo].[Assignment] ([ClassRunId] ASC, [IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Assignment_CourseId_IsDeleted_CreatedDate] ON [dbo].[Assignment] ([CourseId] ASC, [IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Assignment_CreatedBy_IsDeleted_CreatedDate] ON [dbo].[Assignment] ([CreatedBy] ASC, [IsDeleted] ASC, [CreatedDate] DESC)");
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX [IX_Assignment_Type_IsDeleted_CreatedDate] ON [dbo].[Assignment] ([Type] ASC, [IsDeleted] ASC, [CreatedDate] DESC)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }

        private static void RemoveIndexIfExist(MigrationBuilder migrationBuilder)
        {
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Session_IsDeleted_CreatedDate", "Session");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Session_ClassRunId_IsDeleted_CreatedDate", "Session");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Session_CreatedBy_IsDeleted_CreatedDate", "Session");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Session_ClassRunId", "Session");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Session_CreatedBy", "Session");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Session_CreatedDate", "Session");

            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Section_ClassRunId", "Section");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Section_CourseId", "Section");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Section_CreatedBy", "Section");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Section_CreatedDate", "Section");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Section_IsDeleted", "Section");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Section_IsDeleted_CreatedDate", "Section");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Section_ClassRunId_IsDeleted_CreatedDate", "Section");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Section_CourseId_IsDeleted_CreatedDate", "Section");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Section_CreatedBy_IsDeleted_CreatedDate", "Section");

            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_ParticipantAssignmentTrack_IsDeleted_AssignedDate", "ParticipantAssignmentTrack");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_ParticipantAssignmentTrack_IsDeleted_CreatedDate", "ParticipantAssignmentTrack");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_ParticipantAssignmentTrack_IsDeleted_SubmittedDate", "ParticipantAssignmentTrack");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_ParticipantAssignmentTrack_AssignmentId_IsDeleted_CreatedDate", "ParticipantAssignmentTrack");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_ParticipantAssignmentTrack_CreatedBy_IsDeleted_CreatedDate", "ParticipantAssignmentTrack");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_ParticipantAssignmentTrack_RegistrationId_IsDeleted_CreatedDate", "ParticipantAssignmentTrack");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_ParticipantAssignmentTrack_UserId_IsDeleted_CreatedDate", "ParticipantAssignmentTrack");

            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_LectureContent_CreatedBy", "LectureContent");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_LectureContent_CreatedDate", "LectureContent");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_LectureContent_IsDeleted", "LectureContent");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_LectureContent_LectureId", "LectureContent");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_LectureContent_IsDeleted_CreatedDate", "LectureContent");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_LectureContent_CreatedBy_IsDeleted_CreatedDate", "LectureContent");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_LectureContent_LectureId_IsDeleted_CreatedDate", "LectureContent");

            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_AlternativeApprovingOfficerId", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_ContentStatus", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_CourseCode", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_CreatedBy", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_CreatedDate", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_DepartmentId", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_ECertificateTemplateId", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_ExternalCode", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_FirstAdministratorId", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_MOEOfficerId", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_PDAreaThemeId", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_PrimaryApprovingOfficerId", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_SecondAdministratorId", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_Status", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_Status", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_IsDeleted_CreatedDate", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_IsDeleted_ChangedDate", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_IsDeleted_VerifiedDate", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_AlternativeApprovingOfficerId_IsDeleted_CreatedDate", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_ContentStatus_IsDeleted_CreatedDate", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_CourseCode_IsDeleted_CreatedDate", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_ExternalCode_IsDeleted_CreatedDate", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_CreatedBy_IsDeleted_CreatedDate", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_DepartmentId_IsDeleted_CreatedDate", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_ECertificateTemplateId_IsDeleted_CreatedDate", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_FirstAdministratorId_IsDeleted_CreatedDate", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_MOEOfficerId_IsDeleted_CreatedDate", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_PDAreaThemeId_IsDeleted_CreatedDate", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_PrimaryApprovingOfficerId_IsDeleted_CreatedDate", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_SecondAdministratorId_IsDeleted_CreatedDate", "Course");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Course_Status_IsDeleted_CreatedDate", "Course");

            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Registration_ClassRunChangeId", "Registration");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Registration_ClassRunChangeStatus", "Registration");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Registration_ClassRunId", "Registration");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Registration_CourseId", "Registration");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Registration_CreatedBy", "Registration");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Registration_CreatedDate", "Registration");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Registration_IsDeleted", "Registration");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Registration_Status", "Registration");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Registration_UserId", "Registration");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Registration_WithdrawalStatus", "Registration");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Registration_IsDeleted_CreatedDate", "Registration");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Registration_ClassRunChangeId_IsDeleted_CreatedDate", "Registration");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Registration_ClassRunChangeStatus_IsDeleted_CreatedDate", "Registration");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Registration_ClassRunId_IsDeleted_CreatedDate", "Registration");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Registration_CourseId_IsDeleted_CreatedDate", "Registration");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Registration_CreatedBy_IsDeleted_CreatedDate", "Registration");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Registration_Status_IsDeleted_CreatedDate", "Registration");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Registration_UserId_IsDeleted_CreatedDate", "Registration");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Registration_WithdrawalStatus_IsDeleted_CreatedDate", "Registration");

            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Lecture_ClassRunId", "Lecture");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Lecture_CourseId", "Lecture");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Lecture_CreatedBy", "Lecture");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Lecture_CreatedDate", "Lecture");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Lecture_IsDeleted", "Lecture");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Lecture_SectionId", "Lecture");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Lecture_IsDeleted_CreatedDate", "Lecture");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Lecture_ClassRunId_IsDeleted_CreatedDate", "Lecture");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Lecture_CourseId_IsDeleted_CreatedDate", "Lecture");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Lecture_CreatedBy_IsDeleted_CreatedDate", "Lecture");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Lecture_SectionId_IsDeleted_CreatedDate", "Lecture");

            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_CoursePlanningCycle_IsDeleted_CreatedDate", "CoursePlanningCycle");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_CoursePlanningCycle_YearCycle_IsDeleted_CreatedDate", "CoursePlanningCycle");

            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_LearningPathCourse_CourseId", "LearningPathCourse");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_LearningPathCourse_CreatedDate", "LearningPathCourse");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_LearningPathCourse_IsDeleted", "LearningPathCourse");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_LearningPathCourse_LearningPathId", "LearningPathCourse");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_LearningPathCourse_Order", "LearningPathCourse");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_LearningPathCourse_IsDeleted_CreatedDate", "LearningPathCourse");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_LearningPathCourse_CourseId_IsDeleted_CreatedDate", "LearningPathCourse");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_LearningPathCourse_LearningPathId_IsDeleted_CreatedDate", "LearningPathCourse");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_LearningPathCourse_Order_IsDeleted_CreatedDate", "LearningPathCourse");

            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_LearningPath_CreatedBy", "LearningPath");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_LearningPath_CreatedDate", "LearningPath");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_LearningPath_IsDeleted", "LearningPath");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_LearningPath_IsDeleted_CreatedDate", "LearningPath");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_LearningPath_CreatedBy_IsDeleted_CreatedDate", "LearningPath");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_LearningPath_Status_IsDeleted_CreatedDate", "LearningPath");

            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Comment_CreatedDate", "Comment");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Comment_Action_CreatedDate", "Comment");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Comment_ObjectId_CreatedDate", "Comment");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Comment_UserId_CreatedDate", "Comment");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Comment_UserId_ObjectId_CreatedDate", "Comment");

            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_ClassRun_CancellationStatus_IsDeleted_CreatedDate", "ClassRun");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_ClassRun_ClassRunCode_IsDeleted_CreatedDate", "ClassRun");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_ClassRun_ClassRunVenueId_IsDeleted_CreatedDate", "ClassRun");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_ClassRun_ContentStatus_IsDeleted_CreatedDate", "ClassRun");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_ClassRun_CourseId_IsDeleted_CreatedDate", "ClassRun");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_ClassRun_CreatedBy_IsDeleted_CreatedDate", "ClassRun");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_ClassRun_RescheduleStatus_IsDeleted_CreatedDate", "ClassRun");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_ClassRun_Status_IsDeleted_CreatedDate", "ClassRun");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_AttendanceTracking_IsDeleted_CreatedDate", "AttendanceTracking");

            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_AttendanceTracking_RegistrationId_IsDeleted_CreatedDate", "AttendanceTracking");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_AttendanceTracking_SessionId_IsDeleted_CreatedDate", "AttendanceTracking");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_AttendanceTracking_Status_IsDeleted_CreatedDate", "AttendanceTracking");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_AttendanceTracking_Userid_IsDeleted_CreatedDate", "AttendanceTracking");

            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Assignment_IsDeleted_CreatedDate", "Assignment");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Assignment_ClassRunId_IsDeleted_CreatedDate", "Assignment");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Assignment_CourseId_IsDeleted_CreatedDate", "Assignment");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Assignment_CreatedBy_IsDeleted_CreatedDate", "Assignment");
            MigrationHelper.DropIndexIfExists(migrationBuilder, "IX_Assignment_Type_IsDeleted_CreatedDate", "Assignment");
        }
    }
}
