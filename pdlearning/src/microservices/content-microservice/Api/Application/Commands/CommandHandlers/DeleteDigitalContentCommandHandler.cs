using System;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Conexus.Opal.OutboxPattern;
using Microservice.Content.Application.Events;
using Microservice.Content.Application.Models;
using Microservice.Content.Application.Services;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Infrastructure;
using Microservice.Content.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Content.Application.Commands.CommandHandlers
{
    public class DeleteDigitalContentCommandHandler : BaseCommandHandler<DeleteDigitalContentCommand>
    {
        private readonly IRepository<DigitalContent> _digitalContentRepository;
        private readonly IRepository<AttributionElement> _attributionElemRepository;
        private readonly IContentUrlExtractor _contentUrlExtractor;
        private readonly IOutboxQueue _outboxQueue;

        public DeleteDigitalContentCommandHandler(
            IRepository<DigitalContent> digitalContentRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<AttributionElement> attributionElemRepository,
            IAccessControlContext accessControlContext,
            IContentUrlExtractor contentUrlExtractor,
            IOutboxQueue outboxQueue,
            IUserContext userContext) : base(unitOfWorkManager, userContext, accessControlContext)
        {
            _digitalContentRepository = digitalContentRepository;
            _attributionElemRepository = attributionElemRepository;
            _contentUrlExtractor = contentUrlExtractor;
            _outboxQueue = outboxQueue;
        }

        protected override async Task HandleAsync(DeleteDigitalContentCommand command, CancellationToken cancellationToken)
        {
            var dbQuery = _digitalContentRepository
                .GetAllWithAccessControl(AccessControlContext, DigitalContentExpressions.HasOwnerPermissionExpr(CurrentUserId))
                .IgnoreArchivedItems();

            var digitalContent = await dbQuery.FirstOrDefaultAsync(dc => dc.Id == command.Id, cancellationToken);

            if (digitalContent == null)
            {
                throw new ContentAccessDeniedException();
            }

            var model = new DigitalContentModel(digitalContent);
            await _digitalContentRepository.DeleteAsync(command.Id);

            await DeleteRelatedAttributionElement(command.Id, model);

            await _contentUrlExtractor.DeleteExtractedUrls(command.Id);
            await _outboxQueue.QueueMessageAsync(DigitalContentChangeType.Deleted, model, UserContext);
        }

        private async Task DeleteRelatedAttributionElement(Guid digitalContentId, DigitalContentModel model)
        {
            var attributionElements = await _attributionElemRepository.GetAllListAsync(_ => _.DigitalContentId == digitalContentId);

            foreach (var element in attributionElements)
            {
                var attributionElementModel = new AttributionElementModel
                {
                    Id = element.Id,
                    Author = element.Author,
                    DigitalContentId = element.DigitalContentId,
                    LicenseType = element.LicenseType,
                    Source = element.Source,
                    Title = element.Title
                };
                model.AttributionElements.Add(attributionElementModel);

                await _attributionElemRepository.DeleteAsync(element);
            }
        }
    }
}
