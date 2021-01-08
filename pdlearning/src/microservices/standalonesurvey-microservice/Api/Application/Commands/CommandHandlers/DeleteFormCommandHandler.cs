using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Application.Events;
using Microservice.StandaloneSurvey.Application.Services;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.ValueObjects;
using Microservice.StandaloneSurvey.Infrastructure;
using Microservice.StandaloneSurvey.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class DeleteFormCommandHandler : BaseCommandHandler<DeleteFormCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<Domain.Entities.StandaloneSurvey> _formRepository;
        private readonly IRepository<SurveyQuestion> _formQuestionRepository;
        private readonly IRepository<SurveyParticipant> _formParticipantRepository;
        private readonly ISurveyUrlExtractor _surveyUrlExtractor;
        private readonly ICslAccessControlContext _cslAccessControlContext;

        public DeleteFormCommandHandler(
            IThunderCqrs thunderCqrs,
            IRepository<Domain.Entities.StandaloneSurvey> formRepository,
            IRepository<SurveyQuestion> formQuestionRepository,
            IAccessControlContext accessControlContext,
            IRepository<SurveyParticipant> formParticipantRepository,
            ISurveyUrlExtractor surveyUrlExtractor,
            ICslAccessControlContext cslAccessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _thunderCqrs = thunderCqrs;
            _formRepository = formRepository;
            _formQuestionRepository = formQuestionRepository;
            _surveyUrlExtractor = surveyUrlExtractor;
            _cslAccessControlContext = cslAccessControlContext;
            _formParticipantRepository = formParticipantRepository;
        }

        protected override async Task HandleAsync(DeleteFormCommand command, CancellationToken cancellationToken)
        {
            var formQuery = _formRepository.GetAll();

            if (command.SubModule == SubModule.Lna)
            {
                formQuery = formQuery
                    .ApplyAccessControlEx(
                        AccessControlContext,
                        SurveyEntityExpressions.HasOwnerPermissionExpr(this.CurrentUserId))
                    .IgnoreArchivedItems();
            }
            else
            {
                formQuery = formQuery
                    .ApplyCslAccessControl(
                        _cslAccessControlContext,
                        roles: SurveyEntityExpressions.AllManageableCslRoles(),
                        communityId: command.CommunityId,
                        includePredicate: SurveyEntityExpressions.FilterCslSurveyPublishedExpr())
                    .IgnoreArchivedItems();
            }

            var existedForm = await formQuery
                .FirstOrDefaultAsync(p => p.Id == command.FormId, cancellationToken);

            if (existedForm != null)
            {
                await _formParticipantRepository.DeleteAsync(p => p.SurveyId == command.FormId);
                await _formQuestionRepository.DeleteAsync(p => p.SurveyId == command.FormId);
                await _formRepository.DeleteAsync(command.FormId);
                await _surveyUrlExtractor.DeleteExtractedUrls(command.FormId);

                await _thunderCqrs.SendEvent(new SurveyChangeEvent(existedForm, SurveyChangeType.Deleted), cancellationToken);
            }
        }
    }
}
