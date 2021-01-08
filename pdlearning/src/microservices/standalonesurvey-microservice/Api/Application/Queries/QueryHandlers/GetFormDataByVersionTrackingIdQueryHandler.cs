using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Versioning.Entities;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class GetFormDataByVersionTrackingIdQueryHandler : BaseQueryHandler<GetFormDataByVersionTrackingIdQuery, VersionTrackingSurveyDataModel>
    {
        private readonly IRepository<VersionTracking> _versionTrackingRepository;
        private readonly IRepository<Domain.Entities.StandaloneSurvey> _formRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IThunderCqrs _thunderCqrs;

        public GetFormDataByVersionTrackingIdQueryHandler(
            IRepository<VersionTracking> versionTrackingRepository,
            IRepository<AccessRight> accessRightRepositor,
            IRepository<Domain.Entities.StandaloneSurvey> formRepository,
            IAccessControlContext accessControlContext,
            IThunderCqrs thunderCqrs) : base(accessControlContext)
        {
            _thunderCqrs = thunderCqrs;
            _versionTrackingRepository = versionTrackingRepository;
            _formRepository = formRepository;
            _accessRightRepository = accessRightRepositor;
            _thunderCqrs = thunderCqrs;
        }

        protected override async Task<VersionTrackingSurveyDataModel> HandleAsync(GetFormDataByVersionTrackingIdQuery query, CancellationToken cancellationToken)
        {
            var versionTracking = await _versionTrackingRepository.GetAsync(query.VersionTrackingId);
            if (versionTracking == null)
            {
                throw new EntityNotFoundException(typeof(VersionTrackingSurveyDataModel), query.VersionTrackingId);
            }

            var formData = JsonSerializer.Deserialize<VersionTrackingSurveyDataModel>(versionTracking.Data);
            if (formData.FormSections == null)
            {
                if (versionTracking.MajorVersion == 0)
                {
                    return await _thunderCqrs.SendQuery(new GetVersionTrackingFormDataByIdQuery()
                    {
                        FormId = versionTracking.OriginalObjectId,
                        UserId = query.UserId
                    });
                }

                var previousMajorVersion = _versionTrackingRepository.FirstOrDefault(m => m.OriginalObjectId == versionTracking.OriginalObjectId && m.MajorVersion == versionTracking.MajorVersion && m.CanRollback);
                return await _thunderCqrs.SendQuery(new GetVersionTrackingFormDataByIdQuery()
                {
                    FormId = previousMajorVersion.RevertObjectId,
                    UserId = query.UserId
                });
            }

            return formData;
        }
    }
}
