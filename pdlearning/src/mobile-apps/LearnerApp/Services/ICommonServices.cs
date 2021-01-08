using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearnerApp.Models;
using LearnerApp.Models.Course;
using LearnerApp.Models.Learner;
using LearnerApp.Models.MyLearning;
using LearnerApp.Models.Newsfeed;
using LearnerApp.Models.PdCatelogue;

namespace LearnerApp.Services
{
    public interface ICommonServices
    {
        Task<List<ItemCard>> GetBookmarkedCollection(string[] bookmarkType = null, int skipCount = 0, Action<int> totalCount = null);

        Task<List<ItemCard>> GetBookmarkedCommunity(int skipCount = 0, Action<int> totalCount = null);

        Task<List<ItemCard>> GetRecommendationsCollection(string userId, int paging = 1, Action<int> totalCount = null);

        Task<List<ItemCard>> GetNewlyAddedCollection(int page = 1, Action<int> totalCount = null);

        Task<List<ItemCard>> BuildCourseCardList(List<MyCourseSummary> myCourseSummaryList);

        Task<List<ItemCard>> CreateCourseCardList(List<CourseExtendedInformation> myCourseInformationList);

        Task<List<MyDigitalContentSummary>> GetMyDigitalContentBookmarked(int skipCount = 0, Action<int> totalCountAction = null);

        Task<List<MyDigitalContentDetails>> GetMyDigitalContentDetails(string[] digitalContentIds);

        Task<MyDigitalContentDetails> GetMyDigitalContentDetails(string digitalContentId);

        Task<List<ItemCard>> GetRecommendationOrganizationCollection(int pageIndex = 1, Action<int> totalCount = null);

        Task<BookmarkInfo> Bookmark(string id, BookmarkType type, bool isBookmark);

        Task<List<ItemCard>> GetItemCardByGroup(IEnumerable<IGrouping<string, PdCatelogueResource>> resourceTypeGroups);

        Task<List<ClassRun>> GetListMyClassRunDetails(List<MyCourseSummary> myCourseSummary);

        Task<List<ItemCard>> CreateDigitalContentCourseCard(string[] contentIds);

        Task<List<LearningPath>> GetMyLearningPathsBookmarked(int skipCount = 0, Action<int> totalCount = null);

        Task LearningTracking(TrackingEventType eventName, object payload = null);

        Task LearningTrackingStartApp();

        Task<List<MyLectureName>> GetCurrentLectureNameByIds(List<MyCourseSummary> myCourseSummary);

        Task<List<LearningPath>> RecommendationLearningPathsCollection(string userId, int paging = 1, string searchText = "", Action<int> totalCount = null);

        Task<List<Feed>> GetNewsfeed(int skipCount = 0, int maxResultCount = GlobalSettings.MaxResultPerPage, Action<int> count = null);
    }
}
