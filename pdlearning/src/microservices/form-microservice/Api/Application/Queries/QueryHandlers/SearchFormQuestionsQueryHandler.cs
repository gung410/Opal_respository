using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Models;
using Microservice.Form.Common.Extensions;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Infrastructure;
using Microservice.Form.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Application.Queries
{
    public class SearchFormQuestionsQueryHandler : BaseQueryHandler<SearchFormQuestionsQuery, PagedResultDto<FormQuestionModel>>
    {
        private readonly IRepository<FormQuestion> _formQuestionRepository;
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;

        public SearchFormQuestionsQueryHandler(
            IRepository<FormQuestion> formQuestionRepository,
            IAccessControlContext accessControlContext,
            IRepository<AccessRight> accessRightRepository,
            IRepository<FormEntity> formRepository) : base(accessControlContext)
        {
            _formQuestionRepository = formQuestionRepository;
            _formRepository = formRepository;
            _accessRightRepository = accessRightRepository;
        }

        protected override async Task<PagedResultDto<FormQuestionModel>> HandleAsync(SearchFormQuestionsQuery query, CancellationToken cancellationToken)
        {
            var formQuery = _formRepository
                .GetAllWithAccessControl(AccessControlContext, FormEntityExpressions.HasPermissionToSeeFormExpr(CurrentUserId))
                .CombineWithPublicSurveyTemplates(_formRepository)
                .CombineWithAccessRight(_formRepository, _accessRightRepository, CurrentUserId)
                .IgnoreArchivedItems();

            var canUserAccessForm = await formQuery.AnyAsync(p => p.Id == query.FormId, cancellationToken);

            if (!canUserAccessForm)
            {
                throw new FormAccessDeniedException();
            }

            var formQuestionQuery = _formQuestionRepository.GetAll().Where(p => p.FormId == query.FormId);
            var totalCount = await formQuestionQuery.CountAsync(cancellationToken);

            formQuestionQuery = ApplyPaging(formQuestionQuery, query.PagedInfo);

            var entities = await formQuestionQuery
                .OrderBy(question => question.Priority)
                .ThenBy(question => question.MinorPriority)
                .Select(question => new FormQuestionModel(question))
                .ToListAsync(cancellationToken);

            return new PagedResultDto<FormQuestionModel>(totalCount, entities);
        }
    }
}
