using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Content.Application.Commands;
using Microservice.Content.Application.Commands.CommandHandlers;
using Microservice.Content.Versioning.Core;
using Microservice.Content.Versioning.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Content.Versioning.Application.Commands
{
    public class RevertVersionCommandHandler : BaseCommandHandler<RevertVersionCommand>
    {
        private readonly IRepository<VersionTracking> _versionTrackingRepository;

        private readonly IEnumerable<ICheckoutVersionResolver> _checkoutVersionResolvers;

        public RevertVersionCommandHandler(
            IRepository<VersionTracking> versionTrackingRepository,
            IEnumerable<ICheckoutVersionResolver> checkoutVersionResolvers,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IAccessControlContext accessControlContext) : base(unitOfWorkManager, userContext, accessControlContext)
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
