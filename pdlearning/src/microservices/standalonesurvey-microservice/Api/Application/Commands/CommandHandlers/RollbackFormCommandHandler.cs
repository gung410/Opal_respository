using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Application.Events;
using Microservice.StandaloneSurvey.Common.Extensions;
using Microservice.StandaloneSurvey.Common.Helpers;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.ValueObjects;
using Microservice.StandaloneSurvey.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class RollbackFormCommandHandler : BaseCommandHandler<RollbackFormCommand>
    {
        private readonly IRepository<Domain.Entities.StandaloneSurvey> _formRepository;
        private readonly IRepository<SurveyQuestion> _formQuestionRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly ICslAccessControlContext _cslAccessControlContext;
        private readonly IThunderCqrs _thunderCqrs;

        public RollbackFormCommandHandler(
            IRepository<Domain.Entities.StandaloneSurvey> formRepository,
            IRepository<SurveyQuestion> formQuestionRepository,
            IRepository<AccessRight> accessRightRepository,
            ICslAccessControlContext cslAccessControlContext,
            IAccessControlContext accessControlContext,
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _formRepository = formRepository;
            _formQuestionRepository = formQuestionRepository;
            _accessRightRepository = accessRightRepository;
            _cslAccessControlContext = cslAccessControlContext;
            _thunderCqrs = thunderCqrs;
        }

        protected override async Task HandleAsync(RollbackFormCommand command, CancellationToken cancellationToken)
        {
            var formQuery = _formRepository.GetAll();

            if (command.SubModule == SubModule.Lna)
            {
                formQuery = formQuery
                    .ApplyAccessControlEx(
                        AccessControlContext,
                        SurveyEntityExpressions.HasOwnerPermissionExpr(this.CurrentUserId))
                    .CombineWithAccessRight(_formRepository, _accessRightRepository, CurrentUserId);
            }
            else
            {
                formQuery = formQuery
                    .ApplyCslAccessControl(
                        _cslAccessControlContext,
                        roles: SurveyEntityExpressions.AllManageableCslRoles(),
                        communityId: command.CommunityId,
                        includePredicate: SurveyEntityExpressions.FilterCslSurveyPublishedExpr());
            }

            var originalForm = await formQuery.FirstOrDefaultAsync(f => f.Id == command.RevertFromRecordId, cancellationToken);
            if (originalForm == null)
            {
                throw new SurveyAccessDeniedException();
            }

            var newClonedForm = F.DeepClone(originalForm).Pipe(_ =>
            {
                _.Id = command.RevertToRecordId;
                _.ChangedDate = Clock.Now;
                _.IsArchived = false;
                _.ParentId = originalForm.ParentId == Guid.Empty ? originalForm.Id : originalForm.Id;
                _.OriginalObjectId = originalForm.OriginalObjectId == Guid.Empty ? originalForm.ParentId : originalForm.OriginalObjectId;
                return _;
            });

            await _formRepository.InsertAsync(newClonedForm);

            await this.CloneFormQuestions(command.RevertFromRecordId, command.RevertToRecordId);

            await _thunderCqrs.SendEvent(new SurveyChangeEvent(newClonedForm, SurveyChangeType.Rollback), cancellationToken);
        }

        private async Task CloneFormQuestions(Guid originalFormId, Guid newClonnedFormId)
        {
            var allQuestions = await _formQuestionRepository.GetAllListAsync(_ => _.SurveyId == originalFormId);
            var clonedItems = allQuestions
                .Select(q =>
                {
                    q.Id = Guid.NewGuid();
                    q.SurveyId = newClonnedFormId;
                    return q;
                })
                .ToList();

            foreach (var formQuestion in clonedItems)
            {
                await _formQuestionRepository.InsertAsync(formQuestion);
            }
        }
    }
}
