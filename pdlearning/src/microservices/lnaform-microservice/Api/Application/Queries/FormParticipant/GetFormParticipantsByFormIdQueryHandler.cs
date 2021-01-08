using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.LnaForm.Application.Queries
{
    public class GetFormParticipantsByFormIdQueryHandler : BaseQueryHandler<GetFormParticipantsByFormIdQuery, PagedResultDto<FormParticipantModel>>
    {
        private readonly IRepository<FormParticipant> _formParticipantRepository;

        public GetFormParticipantsByFormIdQueryHandler(
            IRepository<FormParticipant> formParticipantRepository,
            IAccessControlContext accessControlContext) : base(accessControlContext)
        {
            _formParticipantRepository = formParticipantRepository;
        }

        protected override async Task<PagedResultDto<FormParticipantModel>> HandleAsync(GetFormParticipantsByFormIdQuery query, CancellationToken cancellationToken)
        {
            var formQuery = this._formParticipantRepository
                .GetAll()
                .Where(formParticipant => formParticipant.FormOriginalObjectId == query.FormOriginalObjectId);
            var totalCount = await formQuery.CountAsync(cancellationToken);
            formQuery = formQuery.OrderByDescending(p => p.ChangedDate != null ? p.ChangedDate.Value : p.CreatedDate);
            formQuery = ApplyPaging(formQuery, query.PagedInfo);

            var entities = await formQuery.Select(formParticipant => new FormParticipantModel(formParticipant)).ToListAsync(cancellationToken);

            return new PagedResultDto<FormParticipantModel>(totalCount, entities);
        }
    }
}
