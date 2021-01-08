using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public sealed class UpdateSurveyResponseStatusCommandHandler : BaseCommandHandler<UpdateSurveyResponseStatusCommand>
    {
        private readonly IRepository<Domain.Entities.StandaloneSurvey> _formRepo;
        private readonly IRepository<SurveyResponse> _surveyResponseRepo;
        private readonly ICslAccessControlContext _cslAccessControlContext;

        public UpdateSurveyResponseStatusCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IAccessControlContext accessControlContext,
            IRepository<Domain.Entities.StandaloneSurvey> formRepo,
            IRepository<SurveyResponse> surveyResponseRepo,
            ICslAccessControlContext cslAccessControlContext) : base(unitOfWorkManager, accessControlContext)
        {
            _formRepo = formRepo;
            _surveyResponseRepo = surveyResponseRepo;
            _cslAccessControlContext = cslAccessControlContext;
        }

        protected override async Task HandleAsync(UpdateSurveyResponseStatusCommand command, CancellationToken cancellationToken)
        {
            if (command.SubModule == SubModule.Lna)
            {
                throw new NotSupportedFeatureException();
            }

            var hasPermission = await _formRepo
                .GetAll()
                .Where(_ => _.Id == command.FormId)
                .ApplyCslAccessControl(
                    _cslAccessControlContext,
                    roles: SurveyEntityExpressions.AllViewableCslRoles(),
                    communityId: command.CommunityId,
                    includePredicate: SurveyEntityExpressions.FilterCslSurveyPublishedExpr())
                .AnyAsync(cancellationToken: cancellationToken);

            if (!hasPermission)
            {
                throw new SurveyAccessDeniedException();
            }

            var response = await _surveyResponseRepo
                                    .GetAll()
                                    .Where(_ => _.FormId == command.FormId && _.UserId == command.UserId)
                                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (response == null)
            {
                throw new ResponseNotFoundException();
            }

            response.AttendanceTime = command.AttendanceTime;
            response.SubmittedTime = command.SubmittedTime;

            await _surveyResponseRepo.UpdateAsync(response);
        }
    }
}
