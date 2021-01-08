using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Content.Application.Models;
using Microservice.Content.Application.Queries.QueryHandlers;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Versioning.Entities;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;
using ContentEntity = Microservice.Content.Domain.Entities.DigitalContent;

namespace Microservice.Content.Application.Queries
{
    public class GetContentByVersionTrackingIdQueryHandler : BaseQueryHandler<GetContentByVersionTrackingIdQuery, DigitalContentModel>
    {
        private readonly IRepository<VersionTracking> _versionTrackingRepository;
        private readonly IRepository<ContentEntity> _formRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IThunderCqrs _thunderCqrs;

        public GetContentByVersionTrackingIdQueryHandler(
            IRepository<VersionTracking> versionTrackingRepository,
            IRepository<AccessRight> accessRightRepositor,
            IRepository<ContentEntity> formRepository,
            IAccessControlContext accessControlContext,
            IThunderCqrs thunderCqrs) : base(accessControlContext)
        {
            _thunderCqrs = thunderCqrs;
            _versionTrackingRepository = versionTrackingRepository;
            _formRepository = formRepository;
            _accessRightRepository = accessRightRepositor;
            _thunderCqrs = thunderCqrs;
        }

        protected override async Task<DigitalContentModel> HandleAsync(GetContentByVersionTrackingIdQuery query, CancellationToken cancellationToken)
        {
            var versionTracking = await _versionTrackingRepository.GetAsync(query.VersionTrackingId);
            if (versionTracking == null)
            {
                throw new EntityNotFoundException(typeof(DigitalContentModel), query.VersionTrackingId);
            }

            var formData = JsonSerializer.Deserialize<DigitalContentModel>(versionTracking.Data);

            return formData;
        }
    }
}
