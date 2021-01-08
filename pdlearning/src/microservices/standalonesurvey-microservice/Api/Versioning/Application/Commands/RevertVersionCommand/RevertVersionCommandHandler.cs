using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Application.Commands;
using Microservice.StandaloneSurvey.Versioning.Core;
using Microservice.StandaloneSurvey.Versioning.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.StandaloneSurvey.Versioning.Application.Commands
{
    public class RevertVersionCommandHandler : BaseCommandHandler<RevertVersionCommand>
    {
        private readonly IRepository<VersionTracking> _versionTrackingRepository;

        private readonly IEnumerable<ICheckoutVersionResolver> _checkoutVersionResolvers;

        public RevertVersionCommandHandler(
            IRepository<VersionTracking> versionTrackingRepository,
            IAccessControlContext accessControlContext,
            IEnumerable<ICheckoutVersionResolver> checkoutVersionResolvers,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _versionTrackingRepository = versionTrackingRepository;
            _checkoutVersionResolvers = checkoutVersionResolvers;
        }

        protected override async Task HandleAsync(RevertVersionCommand command, CancellationToken cancellationToken)
        {
            var trackingData = await _versionTrackingRepository.GetAsync(command.Request.VersionTrackingId);
            var checkoutVersionResolver = _checkoutVersionResolvers.GetCheckoutVersionResolver(command.Request.ObjectType);
            await checkoutVersionResolver.CheckoutVersion(command, trackingData);
        }
    }
}
