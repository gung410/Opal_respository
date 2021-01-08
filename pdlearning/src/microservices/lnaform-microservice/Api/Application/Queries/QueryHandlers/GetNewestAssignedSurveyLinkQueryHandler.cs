using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Application.Services;
using Microservice.LnaForm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Queries
{
    public class GetNewestAssignedSurveyLinkQueryHandler : BaseThunderQueryHandler<GetNewestAssignedSurveyLinkQuery, AssignedLinkModel>
    {
        private readonly IRepository<FormParticipant> _formParticipantRepository;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;

        public GetNewestAssignedSurveyLinkQueryHandler(IRepository<FormParticipant> formParticipantRepository, WebAppLinkBuilder webAppLinkBuilder)
        {
            _formParticipantRepository = formParticipantRepository;
            _webAppLinkBuilder = webAppLinkBuilder;
        }

        protected override Task<AssignedLinkModel> HandleAsync(GetNewestAssignedSurveyLinkQuery query, CancellationToken cancellationToken)
        {
            return _formParticipantRepository
                .GetAll()
                .Where(_ => _.UserId == query.User)
                .OrderByDescending(_ => _.AssignedDate)
                .Select(_ => new AssignedLinkModel(_webAppLinkBuilder.GetFormDetailLink(_.FormId), _.AssignedDate))
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
