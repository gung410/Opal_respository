using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Helper;
using LearnerApp.Models;
using LearnerApp.Models.Course;
using LearnerApp.Models.Learner;
using LearnerApp.Models.MyLearning;
using LearnerApp.Models.Newsfeed;
using LearnerApp.Models.PdCatelogue;
using LearnerApp.Models.UserOnBoarding;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Identity;
using LearnerApp.ViewModels.Base;
using Plugin.Toast;
using Xamarin.Forms;

namespace LearnerApp.Services
{
    public class CommonServices : BaseViewModel, ICommonServices
    {
        private readonly ILearnerBackendService _learnerBackendService;
        private readonly ICourseBackendService _courseBackendService;
        private readonly IPdCatelogueService _pdCatelogueService;
        private readonly IContentBackendService _contentBackendService;
        private readonly IPDPMBackendService _pdpmBackendService;
        private readonly INewsfeedBackendService _newsfeedBackendService;
        private readonly IOrganizationBackendService _organizationBackendService;
        private readonly ICommunityBackendService _communityBackendService;
        private readonly IFormBackendService _formBackendService;

        public CommonServices()
        {
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);
            _courseBackendService = CreateRestClientFor<ICourseBackendService>(GlobalSettings.BackendServiceCourse);
            _pdCatelogueService = CreateRestClientFor<IPdCatelogueService>(GlobalSettings.BackendPdCatelogueService);
            _contentBackendService = CreateRestClientFor<IContentBackendService>(GlobalSettings.BackendServiceContent);
            _pdpmBackendService = CreateRestClientFor<IPDPMBackendService>(GlobalSettings.BackendServicePDPM);
            _newsfeedBackendService = CreateRestClientFor<INewsfeedBackendService>(GlobalSettings.BackendServiceNewsfeed);
            _organizationBackendService = CreateRestClientFor<IOrganizationBackendService>(GlobalSettings.BackendServiceOrganization);
            _communityBackendService = CreateRestClientFor<ICommunityBackendService>(GlobalSettings.BackendServiceSocial);
            _formBackendService = CreateRestClientFor<IFormBackendService>(GlobalSettings.BackendServiceForm);
        }

        public async Task<List<ItemCard>> GetBookmarkedCollection(string[] bookmarkType, int skipCount, Action<int> totalCount)
        {
            var myBookmarkCourses = await ExecuteBackendService(() => _learnerBackendService.GetMyBookmarkedCourses(bookmarkTypeFilter: bookmarkType, skipCount: skipCount));

            if (totalCount != null)
            {
                totalCount?.Invoke(myBookmarkCourses.Payload.TotalCount);
            }

            if (myBookmarkCourses.HasEmptyResult() || myBookmarkCourses.Payload.TotalCount < 1)
            {
                return null;
            }

            var courseCard = await BuildCourseCardList(myBookmarkCourses.Payload.Items);

            foreach (var item in courseCard)
            {
                item.IsVisibleBookmark = PermissionHelper.GetPermissionForBookmark();
            }

            return courseCard;
        }

        public async Task<List<ItemCard>> GetBookmarkedCommunity(int skipCount, Action<int> totalCount)
        {
            var myBookmarkedComunities = await ExecuteBackendService(() => _learnerBackendService.GetMyBookmarkedCommunityInfos(skipCount: skipCount));

            if (totalCount != null)
            {
                totalCount?.Invoke(myBookmarkedComunities.Payload.TotalCount);
            }

            if (myBookmarkedComunities.HasEmptyResult() || myBookmarkedComunities.Payload.TotalCount < 1)
            {
                return null;
            }

            return new List<ItemCard>();
        }

        public async Task<List<ItemCard>> BuildCourseCardList(List<MyCourseSummary> myCourseSummaryList)
        {
            if (myCourseSummaryList.IsNullOrEmpty())
            {
                return new List<ItemCard>();
            }

            var myCourseIdentifiers = myCourseSummaryList.Select(c => c.CourseId).ToArray();

            // From my course identifiers, we need to get more data about a course by calling course API.
            var courseListApiResult = await ExecuteBackendService(() => _courseBackendService.GetCourseListByIdentifiers(myCourseIdentifiers));
            if (courseListApiResult.Payload.IsNullOrEmpty())
            {
                return new List<ItemCard>();
            }

            var classRunList = await GetListMyClassRunDetails(myCourseSummaryList);

            // Get current lecture name
            var myCurrentLectureName = await GetCurrentLectureNameByIds(myCourseSummaryList);

            var courseCardList = CourseCardBuilder.BuildCourseCardListAsync(myCourseSummaryList, courseListApiResult.Payload, classRunList, myCurrentLectureName);

            foreach (var item in courseCardList)
            {
                item.IsVisibleBookmark = PermissionHelper.GetPermissionForBookmark();
            }

            return courseCardList;
        }

        public async Task<List<ItemCard>> GetNewlyAddedCollection(int page, Action<int> totalCount)
        {
            var newlyAddedParam = new PdCatelogueSearchFilter
            {
                Page = page,
                SearchCriteria = new PdSearchCriteria
                {
                    IsArchived = new string[] { "equals", "false", "resourceType:content" },
                    ExpiredDate = new string[] { "gt", DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"), "resourceType:content" },
                    RegistrationMethod = new string[] { "contains", "public", "restricted", "resourceType:course" },
                    ResourceType = new List<string> { "contains", "content", "microlearning", "course" },
                    StartDate = new string[] { "lte", DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"), "resourceType:content" }
                }
            };

            var newlyAddCourses = await ExecuteBackendService(() => _pdCatelogueService.GetNewlyAdded(newlyAddedParam));

            if (newlyAddCourses.HasEmptyResult() || !newlyAddCourses.Payload.Resources.Any())
            {
                return null;
            }

            if (totalCount != null)
            {
                totalCount.Invoke(newlyAddCourses.Payload.Total);
            }

            var resourceTypeGroups = newlyAddCourses.Payload.Resources.GroupBy(p => p.ResourceType);

            var courseCard = await GetItemCardByGroup(resourceTypeGroups);

            if (courseCard != null)
            {
                courseCard.RemoveAll(p => p.Status == StatusCourse.Unpublished.ToString());
            }

            foreach (var item in courseCard)
            {
                item.IsVisibleBookmark = PermissionHelper.GetPermissionForBookmark();
            }

            return courseCard;
        }

        public async Task<List<ItemCard>> GetRecommendationOrganizationCollection(int pageIndex, Action<int> totalCount)
        {
            var recommendation = await ExecuteBackendService(() => _pdpmBackendService.GetRecommendationByOrganization(pageIndex));

            if (recommendation.HasEmptyResult())
            {
                return new List<ItemCard>();
            }

            if (totalCount != null)
            {
                totalCount?.Invoke(recommendation.Payload.TotalItems);
            }

            string[] courseIds = recommendation.Payload.Items.Select(p => p.AdditionalProperties.CourseId).ToArray();

            var courseList = await ExecuteBackendService(() => _courseBackendService.GetCourseListByIdentifiers(courseIds));

            if (courseList.HasEmptyResult())
            {
                return new List<ItemCard>();
            }

            var courseCard = await CreateCourseCardList(courseList.Payload);

            if (!courseCard.IsNullOrEmpty())
            {
                foreach (var item in courseCard)
                {
                    item.IsVisibleBookmark = PermissionHelper.GetPermissionForBookmark();
                }
            }

            return courseCard;
        }

        public async Task<List<ItemCard>> GetRecommendationsCollection(string userId, int paging, Action<int> totalCount)
        {
            var recommendationsParam = new
            {
                UserId = userId,
                Page = paging,
                Limit = GlobalSettings.MaxResultPerPage,
                EnableHighlight = true,
                ResourceTypesFilter = new string[] { "course", "content", "microlearning" }
            };

            var recommendations = await ExecuteBackendService(() => _pdCatelogueService.GetRecommendation(recommendationsParam));

            if (recommendations.HasEmptyResult() || !recommendations.Payload.Resources.Any())
            {
                return null;
            }

            if (totalCount != null)
            {
                totalCount.Invoke(recommendations.Payload.Total);
            }

            var resourceTypeGroups = recommendations.Payload.Resources.GroupBy(p => p.ResourceType);

            var courseCard = await GetItemCardByGroup(resourceTypeGroups);

            if (courseCard != null)
            {
                courseCard.RemoveAll(p => p.Status == StatusCourse.Unpublished.ToString());
            }

            foreach (var item in courseCard)
            {
                item.IsVisibleBookmark = PermissionHelper.GetPermissionForBookmark();
            }

            return courseCard;
        }

        public async Task<List<ItemCard>> CreateCourseCardList(List<CourseExtendedInformation> myCourseInformationList)
        {
            if (myCourseInformationList.IsNullOrEmpty())
            {
                return null;
            }

            var myCourseIdentifiers = myCourseInformationList.Select(c => c.Id).ToArray();

            // From my course identifiers, we need to get more data about a course by calling course API.
            var courseListApiResult = await ExecuteBackendService(() => _learnerBackendService.GetCourseDataFromLearners(new { CourseIds = myCourseIdentifiers }));
            if (courseListApiResult.Payload.IsNullOrEmpty())
            {
                return null;
            }

            var classRunList = await GetListMyClassRunDetails(courseListApiResult.Payload);

            // Get current lecture name
            var myCurrentLectureName = await GetCurrentLectureNameByIds(courseListApiResult.Payload);

            var courseCardList = CourseCardBuilder.BuildCourseCardListAsync(courseListApiResult.Payload, myCourseInformationList, classRunList, myCurrentLectureName);

            foreach (var item in courseCardList)
            {
                item.IsVisibleBookmark = PermissionHelper.GetPermissionForBookmark();
            }

            return courseCardList;
        }

        public async Task<List<MyDigitalContentSummary>> GetMyDigitalContentBookmarked(int skipCount, Action<int> totalCountAction = null)
        {
            var myDigitalContentBookmarked = await _learnerBackendService.GetDigitalContentBookmarked(skipCount: skipCount);

            if (myDigitalContentBookmarked != null)
            {
                if (totalCountAction != null)
                {
                    totalCountAction?.Invoke(myDigitalContentBookmarked.TotalCount);
                }

                return myDigitalContentBookmarked.Items;
            }

            return null;
        }

        public async Task<List<LearningPath>> GetMyLearningPathsBookmarked(int skipCount, Action<int> totalCount = null)
        {
            var myLearningPathsBookmarked = await _learnerBackendService.GetLearningPathsBookmarked(skipCount: skipCount);

            if (myLearningPathsBookmarked != null)
            {
                if (totalCount != null)
                {
                    totalCount?.Invoke(myLearningPathsBookmarked.TotalCount);
                }

                return myLearningPathsBookmarked.Items;
            }

            return null;
        }

        public async Task<List<MyDigitalContentDetails>> GetMyDigitalContentDetails(string[] digitalContentIds)
        {
            var myDigitalContentDetails = await _contentBackendService.GetDigitalContentDetails(digitalContentIds);

            return myDigitalContentDetails;
        }

        public async Task<MyDigitalContentDetails> GetMyDigitalContentDetails(string digitalContentId)
        {
            var myDigitalContentDetails = await _contentBackendService.GetDigitalContentDetails(digitalContentId);

            return myDigitalContentDetails;
        }

        public async Task<BookmarkInfo> Bookmark(string id, BookmarkType type, bool isBookmark)
        {
            var payload = new TrackingBookmark
            {
                IsUnBookmark = !isBookmark,
                ItemId = id,
                ItemType = type.ToString()
            };
            await LearningTracking(TrackingEventType.BookmarkItem, payload);

            if (isBookmark)
            {
                var result = await ExecuteBackendService(() => _learnerBackendService.Bookmark(new { ItemId = id, ItemType = type.ToString() }));

                CrossToastPopUp.Current.ShowToastSuccess("Bookmarked successfully");

                return result.Payload;
            }
            else
            {
                await ExecuteBackendService(() => _learnerBackendService.UnBookmark(id));

                CrossToastPopUp.Current.ShowToastSuccess("Unbookmarked successfully");

                return null;
            }
        }

        public async Task<List<ItemCard>> GetItemCardByGroup(IEnumerable<IGrouping<string, PdCatelogueResource>> resourceTypeGroups)
        {
            var results = new List<ItemCard>();

            foreach (var group in resourceTypeGroups)
            {
                switch (group.Key)
                {
                    case "microlearning":
                    case "course":
                        string[] courseIds = group.Select(p => p.Id).ToArray();
                        var courseList = await ExecuteBackendService(() => _courseBackendService.GetCourseListByIdentifiers(courseIds));

                        if (!courseList.HasEmptyResult())
                        {
                            var courseCard = await CreateCourseCardList(courseList.Payload);
                            if (!courseCard.IsNullOrEmpty())
                            {
                                results.AddRange(courseCard);
                            }
                        }

                        break;
                    case "content":
                        string[] contentIds = group.Select(p => p.Id).ToArray();
                        if (contentIds.Length > 0)
                        {
                            var digitalContentCard = await CreateDigitalContentCourseCard(contentIds);
                            if (!digitalContentCard.IsNullOrEmpty())
                            {
                                results.AddRange(digitalContentCard);
                            }
                        }

                        break;
                    case "learningpath":
                        string[] learningPathIds = group.Select(p => p.Id).ToArray();
                        var learningPaths = await ExecuteBackendService(() => _courseBackendService.GetLearningPathsByIds(learningPathIds));

                        if (!learningPaths.HasEmptyResult())
                        {
                            var learningPathsBookmark = await ExecuteBackendService(() => _learnerBackendService.GetBookmarkedByIds(learningPathIds, ResourceTypesFilter.LearningPathLMM.ToString()));

                            var learningPathCards = learningPaths.Payload.Select(p => new ItemCard
                            {
                                Id = p.Id,
                                Name = p.Title,
                                ThumbnailUrl = p.ThumbnailUrl,
                                MemberCount = p.Courses.Count,
                                BookmarkInfo = learningPathsBookmark?.Payload?.FirstOrDefault(q => q.ItemId == p.Id),
                                CardType = BookmarkType.LearningPath
                            }).ToList();

                            results.AddRange(learningPathCards);
                        }

                        break;
                    case "community":
                        string[] itemIds = group.Select(p => p.Id).ToArray();
                        var communitiesResult = await ExecuteBackendService(() => _communityBackendService.GetCommunityByIds(new GetCommunityByIdRequestModel(itemIds)));
                        if (!communitiesResult.HasEmptyResult())
                        {
                            var bookmarkedCommunitiesResult = await ExecuteBackendService(() => _learnerBackendService.GetBookmarkedByIds(itemIds, ResourceTypesFilter.Community.ToString()));
                            var communityCardList = CommunityCardBuilder.BuildCommunityCardListAsync(communitiesResult.Payload.Results, bookmarkedCommunitiesResult.Payload);

                            if (!communityCardList.IsNullOrEmpty())
                            {
                                results.AddRange(communityCardList);
                            }
                        }

                        break;
                    case "form":
                        string[] formIds = group.Select(p => p.Id).ToArray();
                        var formResult = await ExecuteBackendService(() => _formBackendService.GetFormsByIds(new { FormIds = formIds }));
                        if (!formResult.HasEmptyResult())
                        {
                            var formCard = formResult.Payload.Select(p => new ItemCard
                            {
                                Id = p.Form.Id,
                                Name = p.Form.Title,
                                CardType = BookmarkType.StandAloneForm
                            }).ToList();

                            results.AddRange(formCard);
                        }

                        break;
                    default:
                        break;
                }
            }

            return results;
        }

        public async Task<List<ClassRun>> GetListMyClassRunDetails(List<MyCourseSummary> myCourseSummary)
        {
            List<string> classRunIds = new List<string>();

            foreach (var item in myCourseSummary)
            {
                if (item.MyClassRun != null)
                {
                    classRunIds.Add(item.MyClassRun.ClassRunId);
                }
            }

            var classRunList = await ExecuteBackendService(() => _courseBackendService.GetClassRunByIds(classRunIds));

            if (classRunList.HasEmptyResult())
            {
                return null;
            }

            return classRunList.Payload;
        }

        public async Task<List<ItemCard>> CreateDigitalContentCourseCard(string[] contentIds)
        {
            var digitalContentDetails = await _contentBackendService.GetDigitalContentDetails(contentIds);

            if (digitalContentDetails.IsNullOrEmpty())
            {
                return null;
            }

            string[] originalObjectIds = digitalContentDetails.Select(p => p.OriginalObjectId).ToArray();

            var digitalContentSummary = await ExecuteBackendService(() => _learnerBackendService.GetMyDigitalContentSummary(new { DigitalContentIds = originalObjectIds }));

            if (digitalContentSummary.HasEmptyResult())
            {
                return null;
            }

            return DigitalContentCardBuilder.BuildDigitalContentCardList(digitalContentSummary.Payload, digitalContentDetails);
        }

        public async Task LearningTracking(TrackingEventType eventName, object payload)
        {
            var accountProperties = Application.Current.Properties.GetAccountProperties();

            if (accountProperties != null)
            {
                var param = new UserTracking
                {
                    EventName = eventName.ToString(),
                    Payload = payload,
                    Time = DateTime.UtcNow.ToString("s") + "Z",
                    SessionId = IdentityService.SessionId,
                    UserId = accountProperties.User.Sub
                };
                await ExecuteBackendService(() => _learnerBackendService.UserTracking(param));
            }
        }

        public async Task LearningTrackingStartApp()
        {
            var payload = new
            {
                LoginFromMobile = true,
                ClientId = GlobalSettings.AuthClientId
            };

            await LearningTracking(TrackingEventType.Resume, payload);
        }

        public async Task<List<MyLectureName>> GetCurrentLectureNameByIds(List<MyCourseSummary> myCourseSummary)
        {
            var currentLectureIds = myCourseSummary.Where(p => p.MyCourseInfo != null && !string.IsNullOrEmpty(p.MyCourseInfo.CurrentLecture)).Select(p => p.MyCourseInfo.CurrentLecture).ToArray();

            if (currentLectureIds.IsNullOrEmpty())
            {
                return null;
            }

            var result = await ExecuteBackendService(() => _courseBackendService.GetLectureNameByIds(currentLectureIds));

            if (result.IsError || result.HasEmptyResult())
            {
                return null;
            }

            return result.Payload;
        }

        public async Task<List<LearningPath>> RecommendationLearningPathsCollection(string userId, int paging, string searchText, Action<int> totalCount)
        {
            var searchParam = new
            {
                UserId = userId,
                Page = paging,
                Limit = GlobalSettings.MaxResultPerPage,
                EnableHighlight = true,
                ResourceTypesFilter = new string[] { "learningpath" },
                SearchText = searchText
            };

            var searchRecommendationLearingPath = await ExecuteBackendService(() => _pdCatelogueService.GetRecommendation(searchParam));

            if (searchRecommendationLearingPath.HasEmptyResult() ||
                searchRecommendationLearingPath.Payload.Total == 0 ||
                searchRecommendationLearingPath.Payload.Resources.IsNullOrEmpty())
            {
                totalCount?.Invoke(0);
                return null;
            }
            else
            {
                string[] ids = searchRecommendationLearingPath.Payload.Resources.Select(p => p.Id).ToArray();

                var learningPaths = await ExecuteBackendService(() => _courseBackendService.GetLearningPathsByIds(ids));
                var learningPathsBookmark = await ExecuteBackendService(() => _learnerBackendService.GetBookmarkedByIds(ids, ResourceTypesFilter.LearningPathLMM.ToString()));

                if (learningPathsBookmark.Payload.Count > 0)
                {
                    foreach (var item in learningPaths.Payload)
                    {
                        item.BookmarkInfo = learningPathsBookmark.Payload.FirstOrDefault(p => p.ItemId == item.Id);
                        item.IsFromLMM = true;
                    }
                }

                totalCount?.Invoke(searchRecommendationLearingPath.Payload.Total);
                return learningPaths.Payload;
            }
        }

        public async Task<List<Feed>> GetNewsfeed(int skipCount, int maxResultCount, Action<int> count)
        {
            var newsfeedResult = await ExecuteBackendService(() => _newsfeedBackendService.GetNewsfeed(new { SkipCount = skipCount, MaxResultCount = maxResultCount }));

            count?.Invoke(newsfeedResult.Payload.TotalCount);

            if (!newsfeedResult.HasEmptyResult() && newsfeedResult.Payload.Items.Any())
            {
                var userInfos = new List<UserInformation>();

                // Get user info
                var ids = new List<string>();
                ids.AddRange(newsfeedResult.Payload.Items.Select(p => p.PostedBy).ToArray());
                ids.AddRange(newsfeedResult.Payload.Items.Select(p => p.UserId).ToArray());
                ids.AddRange(newsfeedResult.Payload.Items.Where(p => p.PostForward != null && !string.IsNullOrEmpty(p.PostForward.PostedBy))
                    .Select(p => p.PostForward.PostedBy)
                    .ToArray());

                var userInfoResult = await ExecuteBackendService(() => _organizationBackendService.GetUserInfomation(new { UserCxIds = ids.Distinct() }));
                if (!userInfoResult.HasEmptyResult() && userInfoResult.Payload.Any())
                {
                    foreach (var newsFeed in newsfeedResult.Payload.Items)
                    {
                        newsFeed.PostToInfo = userInfoResult.Payload.FirstOrDefault(p => p.UserCxId.ToUpper().Equals(newsFeed.UserId.ToUpper()));

                        if (!string.IsNullOrEmpty(newsFeed.PostedBy))
                        {
                            newsFeed.PostedByInfo = userInfoResult.Payload.FirstOrDefault(p => p.UserCxId.ToUpper().Equals(newsFeed.PostedBy.ToUpper()));
                        }

                        if (newsFeed.PostForward != null)
                        {
                            newsFeed.PostForward.PostedByInfo = userInfoResult.Payload.FirstOrDefault(p => p.UserCxId.ToUpper().Equals(newsFeed.PostForward.PostedBy.ToUpper()));
                        }
                    }
                }

                return newsfeedResult.Payload.Items;
            }

            return null;
        }
    }
}
