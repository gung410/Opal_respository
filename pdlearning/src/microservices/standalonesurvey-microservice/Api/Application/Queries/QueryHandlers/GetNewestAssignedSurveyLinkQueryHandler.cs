using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Application.Services;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class GetNewestAssignedSurveyLinkQueryHandler : BaseThunderQueryHandler<GetNewestAssignedSurveyLinkQuery, AssignedLinkModel>
    {
        private readonly IRepository<SurveyParticipant> _formParticipantRepository;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;

        public GetNewestAssignedSurveyLinkQueryHandler(IRepository<SurveyParticipant> formParticipantRepository, WebAppLinkBuilder webAppLinkBuilder)
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
                .Select(_ => new AssignedLinkModel(_webAppLinkBuilder.GetStandaloneFormPlayerLink(_.SurveyId, query.SubModule), _.AssignedDate))
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
