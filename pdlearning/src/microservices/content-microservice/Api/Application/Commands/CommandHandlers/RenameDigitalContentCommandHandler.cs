using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Conexus.Opal.OutboxPattern;
using Microservice.Content.Application.Events;
using Microservice.Content.Application.Models;
using Microservice.Content.Common.Extensions;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Infrastructure;
using Microservice.Content.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Commands.CommandHandlers
{
    public class RenameDigitalContentCommandHandler : BaseCommandHandler<RenameDigitalContentCommand>
    {
        private readonly IRepository<DigitalContent> _digitalContentRepository;
        private readonly IRepository<AttributionElement> _attributionElemRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IOutboxQueue _outboxQueue;

        public RenameDigitalContentCommandHandler(
            IRepository<DigitalContent> digitalContentRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<AccessRight> accessRightRepository,
            IAccessControlContext accessControlContext,
            IThunderCqrs thunderCqrs,
            IOutboxQueue outboxQueue,
            IRepository<AttributionElement> attributionElemRepository,
            IUserContext userContext) : base(unitOfWorkManager, userContext, accessControlContext)
        {
            _digitalContentRepository = digitalContentRepository;
            _attributionElemRepository = attributionElemRepository;
            _accessRightRepository = accessRightRepository;
            _outboxQueue = outboxQueue;
            _thunderCqrs = thunderCqrs;
        }

        protected override async Task HandleAsync(RenameDigitalContentCommand command, CancellationToken cancellationToken)
        {
            var dbQuery = _digitalContentRepository
                .GetAllWithAccessControl(AccessControlContext, DigitalContentExpressions.HasOwnerOrApprovalPermissionExpr(command.UserId))
                .CombineWithAccessRight(_digitalContentRepository, _accessRightRepository, CurrentUserId)
                .IgnoreArchivedItems();

            var existedDigitalContent = await dbQuery.FirstOrDefaultAsync(dc => dc.Id == command.Request.Id, cancellationToken);

            if (existedDigitalContent == null)
            {
                throw new ContentAccessDeniedException();
            }

            existedDigitalContent.Title = command.Request.Title;
            var model = new DigitalContentModel(existedDigitalContent);

            var attributionElements = await _attributionElemRepository
                .GetAllListAsync(_ => _.DigitalContentId == command.Request.Id);

            var attributionElementModel = attributionElements
                .Select(a => new AttributionElementModel
                {
                    Id = a.Id,
                    Author = a.Author,
                    DigitalContentId = a.DigitalContentId,
                    LicenseType = a.LicenseType,
                    Source = a.Source,
                    Title = a.Title
                });

            model.AttributionElements.AddRange(attributionElementModel);

            await _digitalContentRepository.UpdateAsync(existedDigitalContent);

            await _outboxQueue.QueueMessageAsync(DigitalContentChangeType.Updated, model, UserContext);
        }
    }
}
