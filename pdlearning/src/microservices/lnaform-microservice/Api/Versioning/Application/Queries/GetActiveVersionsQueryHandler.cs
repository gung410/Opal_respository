using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.LnaForm.Versioning.Application.Model;
using Microservice.LnaForm.Versioning.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Versioning.Application.Queries
{
    public class GetActiveVersionsQueryHandler : BaseThunderQueryHandler<GetActiveVersionsQuery, VersionTrackingModel>
    {
        private readonly IRepository<VersionTracking> _versionTrackingRepository;

        public GetActiveVersionsQueryHandler(IRepository<VersionTracking> versionTrackingRepository)
        {
            _versionTrackingRepository = versionTrackingRepository;
        }

        protected override Task<VersionTrackingModel> HandleAsync(GetActiveVersionsQuery query, CancellationToken cancellationToken)
        {
            return _versionTrackingRepository
                .GetAll()
                .Where(_ => _.OriginalObjectId == query.OriginalObjectId)
                .OrderByDescending(p => p.CreatedDate)
                .Take(1)
                .Select(vt => new VersionTrackingModel()
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
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
