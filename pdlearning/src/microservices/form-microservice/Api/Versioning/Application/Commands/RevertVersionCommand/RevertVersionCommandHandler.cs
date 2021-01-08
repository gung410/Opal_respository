using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Commands;
using Microservice.Form.Versioning.Core;
using Microservice.Form.Versioning.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Form.Versioning.Application.Commands
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
