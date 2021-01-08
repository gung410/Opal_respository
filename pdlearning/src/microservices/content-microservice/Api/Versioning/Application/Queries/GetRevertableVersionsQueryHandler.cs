using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Content.Versioning.Application.Model;
using Microservice.Content.Versioning.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Versioning.Application.Queries
{
    public class GetRevertableVersionsQueryHandler : BaseThunderQueryHandler<GetRevertableVersionsQuery, List<VersionTrackingModel>>
    {
        private readonly IRepository<VersionTracking> _versionTrackingRepository;

        public GetRevertableVersionsQueryHandler(IRepository<VersionTracking> versionTrackingRepository)
        {
            _versionTrackingRepository = versionTrackingRepository;
        }

        protected override Task<List<VersionTrackingModel>> HandleAsync(GetRevertableVersionsQuery query, CancellationToken cancellationToken)
        {
            return _versionTrackingRepository
                .GetAll()
                .Where(_ => (_.OriginalObjectId == query.OriginalObjectId && _.CanRollback))
                .OrderByDescending(p => p.CreatedDate)
                .Select(vt => new VersionTrackingModel
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
                })
                .ToListAsync(cancellationToken);
        }
    }
}
