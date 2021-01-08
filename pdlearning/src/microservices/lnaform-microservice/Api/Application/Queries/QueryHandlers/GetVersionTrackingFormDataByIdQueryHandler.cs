using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Common.Extensions;
using Microservice.LnaForm.Domain.Entities;
using Microservice.LnaForm.Domain.ValueObjects.Form;
using Microservice.LnaForm.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Application.Queries
{
    public class GetVersionTrackingFormDataByIdQueryHandler : BaseQueryHandler<GetVersionTrackingFormDataByIdQuery, VersionTrackingFormDataModel>
    {
        private readonly IRepository<FormQuestion> _formQuestionRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<FormSection> _formSectionRepository;
        private readonly IRepository<FormParticipant> _formParticipantRepository;

        public GetVersionTrackingFormDataByIdQueryHandler(
            IRepository<FormQuestion> formQuestionRepository,
            IRepository<AccessRight> accessRightRepository,
            IRepository<FormEntity> formRepository,
            IRepository<FormSection> formSectionRepository,
            IRepository<FormParticipant> formParticipantRepository,
            IAccessControlContext accessControlContext) : base(accessControlContext)
        {
            _formQuestionRepository = formQuestionRepository;
            _accessRightRepository = accessRightRepository;
            _formRepository = formRepository;
            _formSectionRepository = formSectionRepository;
            _formParticipantRepository = formParticipantRepository;
        }

        protected override async Task<VersionTrackingFormDataModel> HandleAsync(GetVersionTrackingFormDataByIdQuery query, CancellationToken cancellationToken)
        {
            var formQuery = _formRepository
                .GetAll()
                .ApplyAccessControlEx(AccessControlContext, LnaFormEntityExpressions.HasPermissionToSeeFormExpr(CurrentUserId));

            var form = await formQuery
                .Where(p => p.Id == query.FormId)
                .WhereIf(query.OnlyPublished, p => p.Status == FormStatus.Published)
                .FirstOrDefaultAsync(cancellationToken);

            if (form == null)
            {
                throw new FormAccessDeniedException();
            }

            var formQuestions = await _formQuestionRepository
                .GetAll()
                .Where(question => question.FormId == query.FormId)
                .OrderBy(question => question.Priority)
                .ThenBy(question => question.MinorPriority)
                .ToListAsync(cancellationToken);

            var formSections = await _formSectionRepository
                .GetAll()
                .Where(section => section.FormId == query.FormId)
                .ToListAsync();

            return new VersionTrackingFormDataModel(form, formQuestions, formSections);
        }
    }
}
