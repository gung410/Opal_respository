using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.StandaloneSurvey.Versioning.Application.Model;
using Microservice.StandaloneSurvey.Versioning.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Versioning.Application.Queries
{
    public class GetVersionTrackingByObjectIdQueryHandler : BaseThunderQueryHandler<GetVersionTrackingByObjectIdQuery, PagedResultDto<VersionTrackingModel>>
    {
        private readonly IRepository<VersionTracking> _versionTrackingRepository;

        public GetVersionTrackingByObjectIdQueryHandler(IRepository<VersionTracking> versionTrackingRepository)
        {
            _versionTrackingRepository = versionTrackingRepository;
        }

        protected override async Task<PagedResultDto<VersionTrackingModel>> HandleAsync(GetVersionTrackingByObjectIdQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _versionTrackingRepository.GetAll().Where(_ => _.OriginalObjectId == query.Request.OriginalObjectId);
            var totalCount = await dbQuery.CountAsync(cancellationToken);

            dbQuery = dbQuery.OrderByDescending(p => p.CreatedDate);

            dbQuery = ApplyPaging(dbQuery, query.Request.PagedInfo);

            var entities = await dbQuery.Select(vt => new VersionTrackingModel()
            {
                Id = vt.Id,
                ObjectType = vt.ObjectType,
                OriginalObjectId = vt.OriginalObjectId,
                RevertObjectId = vt.RevertObjectId,
                CanRollback = vt.CanRollback,
                ChangedByUserId = vt.ChangedByUserId,
                MajorVersion = vt.MajorVersion,
                MinorVersion = vt.MinorVersion,
                Comment = vt.Comment,
                CreatedDate = vt.CreatedDate
            }).ToListAsync(cancellationToken);

            return new PagedResultDto<VersionTrackingModel>(totalCount, entities);
        }
    }
}
