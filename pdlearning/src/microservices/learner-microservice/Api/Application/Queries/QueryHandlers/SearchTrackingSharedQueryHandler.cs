using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class SearchTrackingSharedQueryHandler : BaseQueryHandler<SearchTrackingSharedQuery, PagedResultDto<TrackingSharedDetailByModel>>
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<DigitalContent> _digitalContentRepository;
        private readonly IRepository<LearnerUser> _userRepository;
        private readonly IRepository<UserSharing> _userSharingRepository;
        private readonly IRepository<UserSharingDetail> _userSharingDetailRepository;

        public SearchTrackingSharedQueryHandler(
            IRepository<Course> courseRepository,
            IRepository<DigitalContent> digitalContentRepository,
            IRepository<LearnerUser> userRepository,
            IRepository<UserSharing> userSharingRepository,
            IRepository<UserSharingDetail> userSharingDetailRepository,
            IUserContext userContext) : base(userContext)
        {
            _courseRepository = courseRepository;
            _digitalContentRepository = digitalContentRepository;
            _userRepository = userRepository;
            _userSharingRepository = userSharingRepository;
            _userSharingDetailRepository = userSharingDetailRepository;
        }

        protected override async Task<PagedResultDto<TrackingSharedDetailByModel>> HandleAsync(SearchTrackingSharedQuery query, CancellationToken cancellationToken)
        {
            var userSharingDetailsQuery = _userSharingDetailRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId);

            var userSharingQuery = await GetListUserSharingInfo(userSharingDetailsQuery, cancellationToken);

            userSharingQuery = ApplySorting(userSharingQuery, query.PageInfo, $"{nameof(UserSharing.CreatedDate)} DESC");

            // Select only 1st item in group, which order by createdDate Desc
            // Select-First wont work AsQueryable
            var itemIdList = await userSharingQuery
               .Select(p => new { p.ItemId, p.Id })
               .ToListAsync(cancellationToken);
            var userSharingIds = itemIdList
               .GroupBy(p => p.ItemId)
               .Select(p => p.First().Id)
               .ToList();
            userSharingQuery = userSharingQuery.Where(p => userSharingIds.Contains(p.Id));

            userSharingQuery = ApplyPaging(userSharingQuery, query.PageInfo);

            var totalCount = userSharingIds.Count();

            var userSharingList = await userSharingQuery
                .ToListAsync(cancellationToken);

            var users = await GetListUserInfo(userSharingList, cancellationToken);

            var courses = await GetListCourseInfo(userSharingList, cancellationToken);

            var digitalContents = await GetListDigitalContentInfo(userSharingList, cancellationToken);

            var trackingSharedList = new List<TrackingSharedDetailByModel>();

            foreach (var userSharingGroup in userSharingList.GroupBy(p => new { p.ItemId, p.ItemType }))
            {
                var sharedByUserIds = userSharingGroup.Select(p => p.CreatedBy).ToList();

                var sharedByUsers = users
                    .Where(p => sharedByUserIds.Contains(p.Id))
                    .Select(p => p.FullName())
                    .ToList();

                foreach (var userSharing in userSharingGroup)
                {
                    if (trackingSharedList.Any(p => p.ItemId == userSharing.ItemId))
                    {
                        continue;
                    }

                    var title = "Title";
                    var thumbnailUrl = "ThumbnailUrl";

                    switch (userSharing.ItemType)
                    {
                        case SharingType.Course:
                        case SharingType.Microlearning:
                            title = courses.First(p => p.Id == userSharing.ItemId).CourseName;
                            thumbnailUrl = courses.First(p => p.Id == userSharing.ItemId).ThumbnailUrl;
                            break;
                        case SharingType.DigitalContent:
                            title = digitalContents.First(p => p.OriginalObjectId == userSharing.ItemId).Title;
                            thumbnailUrl = digitalContents.First(p => p.OriginalObjectId == userSharing.ItemId).FileExtension;
                            break;
                        default:
                            throw new KeyNotFoundException("Sharing Type is not support!");
                    }

                    trackingSharedList.Add(new TrackingSharedDetailByModel
                    {
                        ItemId = userSharing.ItemId,
                        ItemType = (LearningTrackingType)userSharing.ItemType,
                        SharedByUsers = sharedByUsers,
                        Title = title,
                        ThumbnailUrl = thumbnailUrl
                    });
                }
            }

            return new PagedResultDto<TrackingSharedDetailByModel>(totalCount, trackingSharedList);
        }

        private async Task<IQueryable<UserSharing>> GetListUserSharingInfo(IQueryable<UserSharingDetail> userSharingDetailsQuery, CancellationToken cancellationToken)
        {
            var userSharingIds = await userSharingDetailsQuery
                .Select(p => p.UserSharingId)
                .ToListAsync(cancellationToken);

            return _userSharingRepository
                .GetAll()
                .Where(p => p.ItemType == SharingType.Course || p.ItemType == SharingType.Microlearning || p.ItemType == SharingType.DigitalContent)
                .Where(p => userSharingIds.Contains(p.Id));
        }

        private Task<List<LearnerUser>> GetListUserInfo(List<UserSharing> userSharings, CancellationToken cancellationToken)
        {
            var userIds = userSharings
                .Select(p => p.CreatedBy).ToList();

            return _userRepository
                .GetAll()
                .Where(p => userIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
        }

        private Task<List<Course>> GetListCourseInfo(List<UserSharing> userSharingList, CancellationToken cancellationToken)
        {
            var courseIds = userSharingList
                .Where(p => p.ItemType == (SharingType)LearningTrackingType.Course ||
                            p.ItemType == (SharingType)LearningTrackingType.Microlearning)
                .Select(p => p.ItemId)
                .ToList();

            return _courseRepository
                .GetAll()
                .Where(p => courseIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
        }

        private Task<List<DigitalContent>> GetListDigitalContentInfo(List<UserSharing> userSharingList, CancellationToken cancellationToken)
        {
            var digitalContentIds = userSharingList
                .Where(p => p.ItemType == (SharingType)LearningTrackingType.DigitalContent)
                .Select(p => p.ItemId)
                .ToList();

            return _digitalContentRepository
                .GetAll()
                .Where(p => digitalContentIds.Contains(p.OriginalObjectId))
                .ToListAsync(cancellationToken);
        }
    }
}
