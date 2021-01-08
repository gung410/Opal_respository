using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Models;
using Microservice.Form.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Form.Application.Queries
{
    public class GetMyParticipantDataByFormIdQueryHandler : BaseQueryHandler<GetMyParticipantDataByFormIdQuery, FormParticipantModel>
    {
        private readonly IRepository<FormParticipant> _formParticipantRepository;

        public GetMyParticipantDataByFormIdQueryHandler(
            IRepository<FormParticipant> formParticipantRepository,
            IAccessControlContext accessControlContext) : base(accessControlContext)
        {
            _formParticipantRepository = formParticipantRepository;
        }

        protected override async Task<FormParticipantModel> HandleAsync(GetMyParticipantDataByFormIdQuery query, CancellationToken cancellationToken)
        {
            var formQuery = await this._formParticipantRepository
                .FirstOrDefaultAsync(formParticipant => formParticipant.FormOriginalObjectId == query.FormOriginalObjectId && formParticipant.UserId == CurrentUserId);

            return formQuery != null ? new FormParticipantModel(formQuery)
                : null;
        }
    }
}
