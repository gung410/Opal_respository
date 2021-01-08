using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Models;
using LearnerApp.Models.Course;
using LearnerApp.Models.Learner;
using LearnerApp.Models.MyLearning;
using LearnerApp.Models.OutstandingTask;
using LearnerApp.Models.Sharing;
using Refit;

namespace LearnerApp.Services.Backend
{
    public interface ILearnerBackendService
    {
        [Get("/api/me/courses?&statusFilter={statusFilter}&orderBy={orderBy}&maxResultCount={maxResultCount}&skipCount={skipCount}")]
        Task<ListResultDto<MyCourseSummary>> GetMyCourses(LearningCourseType courseType, StatusLearning statusFilter, int maxResultCount = GlobalSettings.MaxResultPerPage, int skipCount = 0, string orderBy = "LastLogin DESC");

        /// <summary>
        /// Get course information of learners.
        /// Don't be confused of "me" because this API will get all course of all learners, not specific for a user.
        /// </summary>
        /// <param name="courseIds">The course identifier list.</param>
        /// <returns>A list of course summary.</returns>
        [Post("/api/me/courses/getByCourseIds")]
        Task<List<MyCourseSummary>> GetCourseDataFromLearners([Body] object courseIds);

        [Get("/api/me/courses/bookmarks?maxResultCount={maxResultCount}&skipCount={skipCount}")]
        Task<ListResultDto<MyCourseSummary>> GetMyBookmarkedCourses([Query(CollectionFormat.Multi)] string[] bookmarkTypeFilter, int skipCount, int maxResultCount = GlobalSettings.MaxResultPerPage);

        [Post("/api/me/courses/enroll")]
        Task<Enroll> CreateCourseEnrollment([Body] object data);

        [Post("/api/me/courses/reEnroll")]
        Task<Enroll> CreateCourseReEnrollment([Body] ReEnrollCourse reEnrollCourse);

        [Delete("/api/me/courses/{myCourseId}")]
        Task DeleteCourseEnrollment(string myCourseId);

        [Put("/api/me/courses/lectures/complete/{myLectureId}")]
        Task CompleteCourseLecture(string myLectureId);

        [Get("/api/me/courses/details/byCourseId/{courseId}")]
        Task<MyCourseSummary> GetMyCourseSummary(string courseId);

        [Post("/api/me/courses/summary")]
        Task<List<CourseSummary>> GetNumberOfStatusCourse([Body] object statusFilter);

        [Get("/api/me/courses?courseType={courseType}&maxResultCount={maxResultCount}&statusFilter={statusFilter}&skipCount={skipCount}&orderBy={orderBy}")]
        Task<ListResultDto<MyCourseSummary>> GetMyLearningCourse(string courseType, MyLearningStatus statusFilter, int maxResultCount = GlobalSettings.MaxResultPerPage, int skipCount = 0, string orderBy = "LastLogin DESC");

        [Post("/api/me/digitalcontent/search")]
        Task<MyLearningSearchResultDto<MyDigitalContentSummary>> GetMyLearningDigitalContent([Body] object param);

        [Get("/api/me/digitalcontent/details/{digitalContentId}")]
        Task<MyDigitalContentSummary> GetDigitalContentDetails(string digitalContentId);

        [Post("/api/me/digitalcontent/enroll")]
        Task PostEnrollDigitalContent([Body] object param);

        [Get("/api/me/bookmarks/digitalcontent?&maxResultCount={maxResultCount}&skipCount={skipCount}")]
        Task<ListResultDto<MyDigitalContentSummary>> GetDigitalContentBookmarked(int maxResultCount = GlobalSettings.MaxResultPerPage, int skipCount = 0);

        [Get("/api/me/bookmarks/learningpath?itemType=LearningPath&maxResultCount={maxResultCount}&skipCount={skipCount}")]
        Task<ListResultDto<LearningPath>> GetLearningPathsBookmarked(int maxResultCount = GlobalSettings.MaxResultPerPage, int skipCount = 0);

        [Get("/api/reviews?maxResultCount={maxResultCount}&skipCount={skipCount}&itemId={itemId}&itemTypeFilter={itemTypeFilter}&orderBy={orderBy}")]
        Task<ListResultDto<UserReview>> GetUserReviews(string itemId, PdActivityType itemTypeFilter, string orderBy = "CreatedDate desc", int maxResultCount = GlobalSettings.MaxResultPerPage, int skipCount = 0);

        [Get("/api/reviews/me?itemId={itemId}&itemType={itemType}")]
        Task<UserReview> GetCurrentUserReview(string itemId, PdActivityType itemType);

        [Post("/api/reviews/create")]
        Task<UserReview> CreateUserReview([Body] object itemReview);

        [Put("/api/reviews/{itemId}")]
        Task<UserReview> UpdateUserReview(string itemId, [Body] object itemReview);

        [Get("/api/me/assignments?registrationId={registrationId}")]
        Task<ListResultDto<Assignment>> GetMyAssignmentsByRegistrationId(string registrationId);

        [Post("/api/me/assignments/changeStatus")]
        Task UpdateAssignmentStatus([Body] object item);

        [Put("/api/me/courses/updateStatus")]
        Task<MyCourseInfo> UpdateCourseStatus([Body] object request);

        [Post("/api/me/digitalcontent/update")]
        Task<MyDigitalContentSummary> UpdateDigitalContentStatus([Body] object request);

        [Post("/api/me/digitalcontent/getIds")]
        Task<List<MyDigitalContentSummary>> GetMyDigitalContentSummary([Body] object param);

        [Post("/api/me/bookmarks/create")]
        Task<BookmarkInfo> Bookmark([Body] object param);

        [Delete("/api/me/bookmarks/unbookmarkItem/{id}")]
        Task UnBookmark(string id);

        [Get("/api/me/learningpaths/detail/{learningPathId}")]
        Task<LearningPath> GetLearningPathById(string learningPathId);

        [Post("/api/me/learningpaths")]
        Task<LearningPath> CreateLearningPath([Body] LearningPath learningPath);

        [Post("/api/me/learningpaths/search/ids")]
        Task<List<LearningPath>> GetLearningPathsByLearningPathsIds([Body] string[] ids);

        [Put("/api/me/learningpaths")]
        Task<LearningPath> UpdateLearningPath(LearningPath learningPath);

        [Delete("/api/me/learningpaths/{learningPathId}")]
        Task DeleteLearningPath(string learningPathId);

        [Post("/api/userSharing")]
        Task<LearningPathSharing> CreateShareLearningPath([Body] object param);

        [Put("/api/userSharing")]
        Task UpdateSharedLearningPath([Body] object param);

        [Get("/api/userSharing/shared/me/learningpath?maxResultCount={maxResultCount}&skipCount={skipCount}")]
        Task<ListResultDto<LearningPath>> GetLearningPathsSharedToMe(int skipCount, int maxResultCount = GlobalSettings.MaxResultPerPage);

        [Get("/api/userSharing/details/byItemId/{itemId}")]
        Task<LearningPathSharing> GetLearningPathsSharedList(string itemId);

        [Get("/api/me/bookmarks/ids?ItemType={itemType}")]
        Task<List<BookmarkInfo>> GetBookmarkedByIds([Query(CollectionFormat.Multi)] string[] itemIds, string itemType);

        [Get("/api/me/bookmarks?ItemType=Community&maxResultCount={maxResultCount}&skipCount={skipCount}")]
        Task<ListResultDto<BookmarkInfo>> GetMyBookmarkedCommunityInfos(int skipCount = 0, int maxResultCount = GlobalSettings.MaxResultPerPage);

        [Get("/api/me/learningpaths/searchUsers?SearchText={searchText}&maxResultCount={maxResultCount}&skipCount={skipCount}")]
        Task<ListResultDto<UserSharing>> SearchUsers(string searchText, int skipCount, int maxResultCount = GlobalSettings.MaxResultPerPage, CancellationToken ctoken = default(CancellationToken));

        [Post("/api/me/learningpaths/enablePublic/{id}")]
        Task PublicLearningPath(string id);

        [Post("/api/userTracking")]
        Task UserTracking([Body] UserTracking trackingParam);

        [Post("/api/me/courses/search")]
        Task<MyLearningSearchResultDto<MyCourseSummary>> MyLearningCourseSearch([Body] object param);

        [Post("/api/me/learningpaths/search")]
        Task<MyLearningSearchResultDto<LearningPath>> SearchLearningPath([Body] object param);

        [Post("/api/userPreferences/get")]
        Task<List<UserPreference>> GetUserPreference([Body] object param);

        [Put("/api/userPreferences")]
        Task UpdateUserPreference([Body] object[] param);

        [Get("/api/me/outstandingTasks?maxResultCount={maxResultCount}&skipCount={skipCount}")]
        Task<ListResultDto<OutstandingTask>> GetOutstandingTasks(int skipCount = 0, int maxResultCount = GlobalSettings.MaxResultPerPage);

        [Get("/api/me/outstandingTasks/byId/{id}")]
        Task<OutstandingTask> GetOutstandingTaskById(string id);

        [Post("/api/userTracking/share")]
        Task ShareContent([Body] ShareContentArgumentsPayload param);

        [Get("/api/userTracking/share/get?maxResultCount={maxResultCount}&skipCount={skipCount}")]
        Task<ListResultDto<SharingContentItem>> GetShares(int skipCount = 0, int maxResultCount = GlobalSettings.MaxResultPerPage);

        [Post("/api/userTracking/trackingInfo/byItemId")]
        Task<TrackingInfo> GetTrackingInfo([Body] object param);

        [Post("/api/userTracking/like")]
        Task<TrackingInfo> Like([Body] object param);
    }
}
