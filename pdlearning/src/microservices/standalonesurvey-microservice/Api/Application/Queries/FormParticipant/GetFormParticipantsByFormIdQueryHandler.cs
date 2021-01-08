using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class GetFormParticipantsByFormIdQueryHandler : BaseQueryHandler<GetFormParticipantsByFormIdQuery, PagedResultDto<SurveyParticipantModel>>
    {
        private readonly IRepository<SurveyParticipant> _formParticipantRepository;

        public GetFormParticipantsByFormIdQueryHandler(
            IRepository<SurveyParticipant> formParticipantRepository,
            IAccessControlContext accessControlContext) : base(accessControlContext)
        {
            _formParticipantRepository = formParticipantRepository;
        }

        protected override async Task<PagedResultDto<SurveyParticipantModel>> HandleAsync(GetFormParticipantsByFormIdQuery query, CancellationToken cancellationToken)
        {
            var formQuery = this._formParticipantRepository
                .GetAll()
                .Where(formParticipant => formParticipant.SurveyOriginalObjectId == query.FormOriginalObjectId);
            var totalCount = await formQuery.CountAsync(cancellationToken);
            formQuery = formQuery.OrderByDescending(p => p.ChangedDate != null ? p.ChangedDate.Value : p.CreatedDate);
            formQuery = ApplyPaging(formQuery, query.PagedInfo);

            var entities = await formQuery.Select(formParticipant => new SurveyParticipantModel(formParticipant)).ToListAsync(cancellationToken);

            return new PagedResultDto<SurveyParticipantModel>(totalCount, entities);
        }
    }
}
