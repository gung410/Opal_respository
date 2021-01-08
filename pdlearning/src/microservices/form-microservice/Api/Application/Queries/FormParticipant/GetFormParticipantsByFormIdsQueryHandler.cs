using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Models;
using Microservice.Form.Common.Extensions;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Domain.ValueObjects.Form;
using Microservice.Form.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Application.Queries
{
    public class GetFormParticipantsByFormIdsQueryHandler : BaseQueryHandler<GetFormParticipantsByFormIdsQuery, IEnumerable<FormParticipantFormModel>>
    {
        private readonly IRepository<FormParticipant> _formParticipantRepository;
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;

        public GetFormParticipantsByFormIdsQueryHandler(
            IRepository<FormParticipant> formParticipantRepository,
            IRepository<AccessRight> accessRightRepository,
            IRepository<FormEntity> formRepository,
            IAccessControlContext accessControlContext) : base(accessControlContext)
        {
            _formParticipantRepository = formParticipantRepository;
            _formRepository = formRepository;
            _accessRightRepository = accessRightRepository;
        }

        protected override async Task<IEnumerable<FormParticipantFormModel>> HandleAsync(GetFormParticipantsByFormIdsQuery query, CancellationToken cancellationToken)
        {
            if (!query.FormIds.Any())
            {
                return new List<FormParticipantFormModel>();
            }

            // Check access control for current user
            var formQuery = _formRepository
                .GetAllWithAccessControl(AccessControlContext, FormEntityExpressions.HasPermissionToSeeFormExpr(CurrentUserId))
                .CombineWithAccessRight(_formRepository, _accessRightRepository, CurrentUserId);

            // Get list form info with form participants (if has)
            var forms = await formQuery
                .Where(p => query.FormIds.Distinct().Contains(p.Id) && p.Status == FormStatus.Published && !p.IsArchived)
                .GroupJoin(
                    this._formParticipantRepository.GetAll().Where(p => p.UserId == CurrentUserId),
                    form => form.OriginalObjectId,
                    formParticipant => formParticipant.FormOriginalObjectId,
                    (form, formParticipant) => new { form, formParticipant })
                .SelectMany(p => p.formParticipant.DefaultIfEmpty(), (gj, formParticipant) => new FormParticipantFormModel(gj.form, formParticipant))
                .ToListAsync(cancellationToken);

            return forms;
        }
    }
}
