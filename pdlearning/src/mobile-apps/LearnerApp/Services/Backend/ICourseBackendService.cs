using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using LearnerApp.Models;
using LearnerApp.Models.Achievement;
using LearnerApp.Models.Course;
using LearnerApp.Models.Learner;
using LearnerApp.Models.MyLearning;
using Refit;

namespace LearnerApp.Services.Backend
{
    public interface ICourseBackendService
    {
        [Get("/api/learningcontent/{courseId}/toc?includeAdditionalInfo=true")]
        Task<List<TableOfContent>> GetTableOfContentByCourseId(string courseId);

        [Get("/api/learningcontent/{courseId}/toc?classRunId={classRunId}&includeAdditionalInfo=true")]
        Task<List<TableOfContent>> GetTableOfContentByClassRunId(string courseId, string classRunId);

        [Post("/api/learningcontent/lectures/getAllNamesByListIds")]
        Task<List<MyLectureName>> GetLectureNameByIds([Body] string[] ids);

        [Get("/api/courses/{courseId}")]
        Task<CourseExtendedInformation> GetCourseByCourseId(string courseId);

        [Get("/api/courses/recent?SkipCount={skipCount}&MaxResultCount={maxResultCount}")]
        Task<ListResultDto<CourseInformation>> GetNewlyAddedCourses(int skipCount = 0, int maxResultCount = 5);

        [Get("/api/courses/search?searchText={searchText}&SearchStatus={searchStatus}&skipCount={skipCount}&maxResultCount={maxResultCount}&withinCopyrightDuration=true")]
        Task<ListResultDto<CourseInformation>> SearchCourses(string searchText, string searchStatus = "Published", int skipCount = 0, int maxResultCount = 5);

        [Post("/api/courses/getByIds")]
        Task<List<CourseExtendedInformation>> GetCourseListByIdentifiers([Body] string[] courseIds);

        [Post("/api/classrun/byCourseId")]
        Task<ListResultDto<ClassRun>> GetClassRuns(ClassRunRequest classRunRequest);

        [Get("/api/classrun/{classRunId}")]
        Task<ClassRun> GetClassRunById(string classRunId);

        [Post("/api/classrun/getClassRunsByIds")]
        Task<List<ClassRun>> GetClassRunByIds([Body] object param);

        [Post("/api/registration/createRegistration")]
        Task<List<Registration>> ApplyClassRun([Body] ClassRunRegistration classRun);

        [Put("/api/registration/changeStatusByCourseClassRun")]
        Task ChangeStatusByCourseClassRun([Body] object classRun);

        [Get("/api/classrun/checkClassIsFull")]
        Task<bool> CheckClassIsFull(string classRunId);

        [Put("/api/registration/changeWithdrawStatus")]
        Task Withdraw([Body] Withdrawal withdraw);

        [Post("/api/courses/search")]
        Task<ListResultDto<PrerequisiteCourse>> GetPreRequisites([Body] object preRequisite);

        [Post("/api/assignment/getAssignmentByIds")]
        Task<List<AssignmentInfo>> GetMyAssignmentsByAssignmentIds([Body] AssignmentRequest assignment);

        [Post("/api/session/byClassRunIds")]
        Task<List<Session>> GetSessionsByClassRunIds([Body] string[] classRunIds);

        [Post("/api/registration/changeClassRun")]
        Task ChangeClassRun([Body] ClassRunChangeRequest classRunChange);

        [Put("/api/attendancetracking/changeReason")]
        Task ChangeAbsenceReason([Body] AbsenceReason absenceReason);

        [Get("/api/attendancetracking/currentUser/{classRunId}")]
        Task<List<AttendanceTracking>> GetUserAttendanceTrackingByClassRunId(string classRunId);

        [Put("/api/attendancetracking/learnerTakeAttendance")]
        Task<AttendanceTracking> TakeAttendance([Body] object sessionCode);

        [Post("/api/participantAssignmentTrack/getParticipantAssignmentTracks")]
        Task<ListResultDto<ParticipantAssignmentTrack>> GetParticipantAssignmentTracks(ParticipantAssignmentTrackRequest participantAssignmentTrackRequest);

        [Post("/api/participantAssignmentTrack/getByIds")]
        Task<List<ParticipantAssignmentTrack>> GetParticipantAssignmentTracksByIds(string[] ids);

        [Post("/api/course/comment/search")]
        Task<ListResultDto<Comment>> GetAssignmentComment([Body] CommentRequest commentRequest);

        [Post("/api/course/comment/create")]
        Task<Comment> CreateAssignmentComment([Body] CreateCommentRequest commentRequest);

        [Get("/api/registration/getLearnerCourseViolation?courseId={courseId}&classrunId={classrunId}")]
        Task<Violation> GetCourseViolation(string courseId, string classrunId);

        [Put("/api/registration/{registrationId}/completePostEvaluation")]
        Task CompletePostEvaluation(string registrationId);

        [Post("/api/learningpaths/getByIds")]
        Task<List<LearningPath>> GetLearningPathsByIds([Body] string[] ids);

        [Post("/api/participantAssignmentTrack/getByIds")]
        Task<List<ParticipantAssignmentTrack>> ParticipantAssignmentTrackByIds([Body] string[] ids);

        [Get("/api/learningcontent/lectures/{lectureId}")]
        Task<LectureResourceDetails> GetLectureResourceInfo(string lectureId);

        [Post("/api/classrun/getTotalParticipantInClassRun")]
        Task<List<Participant>> GetTotalParticipantInClassRun(object request);

        [Get("/api/classrun/checkClassIsFull?classRunId={classRunId}")]
        Task<bool> CheckClassFull(string classRunId);

        [Get("/api/ecertificate/{registrationId}/download-ecertificate?fileFormat={fileFormat}")]
        Task<HttpContent> DownloadECertificate(string registrationId, string fileFormat);

        [Get("/api/registration/getMyCertificates?SkipCount={skipCount}&MaxResultCount={maxResultCount}")]
        Task<ListResultDto<AchievementECertificateDto>> GetMyAwardedECertificates(
            int skipCount,
            int maxResultCount);
    }
}
