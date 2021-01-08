using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Common.Extensions;
using Microservice.LnaForm.Domain.Entities;
using Microservice.LnaForm.Domain.ValueObjects.Form;
using Microservice.LnaForm.Infrastructure;
using Microservice.LnaForm.Versioning.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Cqrs;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Application.Queries
{
    public class GetFormDataByVersionTrackingIdQueryHandler : BaseQueryHandler<GetFormDataByVersionTrackingIdQuery, VersionTrackingFormDataModel>
    {
        private readonly IRepository<VersionTracking> _versionTrackingRepository;
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IThunderCqrs _thunderCqrs;

        public GetFormDataByVersionTrackingIdQueryHandler(
            IRepository<VersionTracking> versionTrackingRepository,
            IRepository<AccessRight> accessRightRepositor,
            IRepository<FormEntity> formRepository,
            IAccessControlContext accessControlContext,
            IThunderCqrs thunderCqrs) : base(accessControlContext)
        {
            _thunderCqrs = thunderCqrs;
            _versionTrackingRepository = versionTrackingRepository;
            _formRepository = formRepository;
            _accessRightRepository = accessRightRepositor;
            _thunderCqrs = thunderCqrs;
        }

        protected override async Task<VersionTrackingFormDataModel> HandleAsync(GetFormDataByVersionTrackingIdQuery query, CancellationToken cancellationToken)
        {
            var versionTracking = await _versionTrackingRepository.GetAsync(query.VersionTrackingId);
            if (versionTracking == null)
            {
                throw new EntityNotFoundException(typeof(VersionTrackingFormDataModel), query.VersionTrackingId);
            }

            var formData = JsonSerializer.Deserialize<VersionTrackingFormDataModel>(versionTracking.Data);
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
