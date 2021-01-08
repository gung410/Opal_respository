using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Content.Application.Models;
using Microservice.Content.Common.Extensions;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Domain.ValueObject;
using Microservice.Content.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Content.Application.Queries.QueryHandlers
{
    public class GetDigitalContentByIdQueryHandler : BaseQueryHandler<GetDigitalContentByIdQuery, DigitalContentModel>
    {
        private readonly IRepository<DigitalContent> _digitalContentRepository;
        private readonly IRepository<AttributionElement> _attributionElemRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<LearningTracking> _learningTrackingRepository;
        private readonly IRepository<Chapter> _chapterRepository;
        private readonly IRepository<ChapterAttachment> _chapterAttachmentsRepository;

        public GetDigitalContentByIdQueryHandler(
            IRepository<DigitalContent> digitalContentRepository,
            IRepository<AccessRight> accessRightRepository,
            IRepository<LearningTracking> learningTrackingRepository,
            IAccessControlContext accessControlContext,
            IRepository<AttributionElement> attributionElemRepository,
            IRepository<Chapter> chapterRepository,
            IRepository<ChapterAttachment> chapterAttachmentsRepository) : base(accessControlContext)
        {
            _digitalContentRepository = digitalContentRepository;
            _attributionElemRepository = attributionElemRepository;
            _accessRightRepository = accessRightRepository;
            _learningTrackingRepository = learningTrackingRepository;
            _chapterRepository = chapterRepository;
            _chapterAttachmentsRepository = chapterAttachmentsRepository;
        }

        protected override async Task<DigitalContentModel> HandleAsync(GetDigitalContentByIdQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _digitalContentRepository.GetAll().Where(dc => dc.Id == query.Id);

            if (!(await dbQuery.AnyAsync(cancellationToken)))
            {
                throw new ContentNotAvailableException();
            }

            dbQuery = dbQuery
                .ApplyAccessControl(AccessControlContext, DigitalContentExpressions.HasPermissionToSeeContentExpr(query.UserId))
                .CombineWithAccessRight(_digitalContentRepository, _accessRightRepository, CurrentUserId);

            var digitalContentEntity = await dbQuery.FirstOrDefaultAsync(cancellationToken);
            if (digitalContentEntity == null)
            {
                throw new ContentAccessDeniedException();
            }

            var digitalContentModel = new DigitalContentModel(digitalContentEntity);

            var allAttributionElem = await _attributionElemRepository
                .GetAll()
                .Where(p => p.DigitalContentId == digitalContentModel.Id)
                .Select(p => new AttributionElementModel
                {
                    Id = p.Id,
                    DigitalContentId = p.DigitalContentId,
                    Source = p.Source,
                    Author = p.Author,
                    Title = p.Title,
                    LicenseType = p.LicenseType
                })
                .ToListAsync(cancellationToken);

            var learningTracking = await _learningTrackingRepository
                .GetAll()
                .Where(p => p.ItemId == digitalContentModel.Id)
                .Where(p => p.TrackingType == LearningTrackingType.DigitalContent)
                .Select(p => new
                {
                    p.TrackingAction,
                    p.TotalCount
                })
                .ToDictionaryAsync(p => p.TrackingAction, cancellationToken);

            var viewsCount = learningTracking.ContainsKey(LearningTrackingAction.View)
                ? learningTracking[LearningTrackingAction.View].TotalCount
                : 0;

            var downloadsCount = learningTracking.ContainsKey(LearningTrackingAction.DownloadContent)
                ? learningTracking[LearningTrackingAction.DownloadContent].TotalCount
                : 0;

            var chapters = await _chapterRepository
                .GetAllIncluding(_ => _.Attachments)
                .Where(_ => _.ObjectId == digitalContentModel.Id)
                .Select(p => new ChapterModel(p))
                .ToListAsync(cancellationToken);

            digitalContentModel.AttributionElements = allAttributionElem;
            digitalContentModel.Chapters = chapters;
            digitalContentModel.WithViewsCount(viewsCount);
            digitalContentModel.WithDownloadsCount(downloadsCount);

            return digitalContentModel;
        }
    }
}
