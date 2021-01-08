using System.Threading;
using System.Threading.Tasks;
using Microservice.Content.Versioning.Application.Model;
using Microservice.Content.Versioning.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Versioning.Application.Queries
{
    public class GetVersionTrackingByIdQueryHandler : BaseThunderQueryHandler<GetVersionTrackingByIdQuery, VersionTrackingModel>
    {
        private readonly IRepository<VersionTracking> _versionTrackingRepository;

        public GetVersionTrackingByIdQueryHandler(IRepository<VersionTracking> versionTrackingRepository)
        {
            _versionTrackingRepository = versionTrackingRepository;
        }

        protected override async Task<VersionTrackingModel> HandleAsync(GetVersionTrackingByIdQuery query, CancellationToken cancellationToken)
        {
            var versionTracking = await this._versionTrackingRepository.GetAsync(query.VersionId);
            return new VersionTrackingModel
            {
                Id = versionTracking.Id,
                ChangedByUserId = versionTracking.ChangedByUserId,
                Comment = versionTracking.Comment,
                CreatedDate = versionTracking.CreatedDate,
                OriginalObjectId = versionTracking.OriginalObjectId,
                CanRollback = versionTracking.CanRollback,
                RevertObjectId = versionTracking.RevertObjectId,
                ObjectType = versionTracking.ObjectType,
                MajorVersion = versionTracking.MajorVersion,
                MinorVersion = versionTracking.MinorVersion
            };
        }
    }
}
