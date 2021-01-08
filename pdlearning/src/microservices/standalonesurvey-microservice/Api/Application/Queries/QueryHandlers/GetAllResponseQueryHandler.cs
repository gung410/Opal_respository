using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class GetAllResponseQueryHandler : BaseQueryHandler<GetAllResponseQuery, PagedResultDto<ResponsesModel>>
    {
        private readonly IRepository<SyncedUser> _userRepo;
        private readonly IRepository<Domain.Entities.StandaloneSurvey> _formRepo;
        private readonly ICslAccessControlContext _cslAccessControlContext;
        private readonly IRepository<SurveyResponse> _surveyResponseRepo;

        public GetAllResponseQueryHandler(
            IAccessControlContext accessControlContext,
            IRepository<SyncedUser> userRepo,
            IRepository<Domain.Entities.StandaloneSurvey> formRepo,
            ICslAccessControlContext cslAccessControlContext,
            IRepository<SurveyResponse> surveyResponseRepo) : base(accessControlContext)
        {
            _userRepo = userRepo;
            _formRepo = formRepo;
            _cslAccessControlContext = cslAccessControlContext;
            _surveyResponseRepo = surveyResponseRepo;
        }

        protected override async Task<PagedResultDto<ResponsesModel>> HandleAsync(GetAllResponseQuery query, CancellationToken cancellationToken)
        {
            if (query.SubModule == SubModule.Lna)
            {
                throw new NotSupportedFeatureException();
            }

            var hasPermission = await _formRepo
                        .GetAll()
                        .Where(_ => _.Id == query.FormId)
                        .ApplyCslAccessControl(
                            _cslAccessControlContext,
                            roles: SurveyEntityExpressions.AllManageableCslRoles(),
                            communityId: query.CommunityId,
                            includePredicate: SurveyEntityExpressions.FilterCslSurveyPublishedExpr())
                        .AnyAsync(cancellationToken: cancellationToken);

            if (!hasPermission)
            {
                throw new SurveyAccessDeniedException();
            }

            var responseQuery = _surveyResponseRepo
                                    .GetAll()
                                    .Where(_ => _.FormId == query.FormId);

            var total = await responseQuery.CountAsync(cancellationToken);

            responseQuery = ApplyPaging(responseQuery, query.PagedInfo);

            var responses = await responseQuery.Join(
                                        _userRepo.GetAll(),
                                        r => r.UserId,
                                        u => u.Id,
                                        (r, u) => new
                                        {
                                            r.AttendanceTime,
                                            r.SubmittedTime,
                                            FullName = u.FullName(),
                                            u.Email,
                                            PlaceOfWork = u.DepartmentName
                                        })
                                    .ToListAsync(cancellationToken: cancellationToken);

            var result = responses.Select(_ => new ResponsesModel
                                                {
                                                    Email = _.Email,
                                                    PlaceOfWork = _.PlaceOfWork,
                                                    MemberName = _.FullName,
                                                    Attendance = _.AttendanceTime.HasValue ? "1/1" : "0/1",
                                                    Status = _.SubmittedTime.HasValue ? "completed" : "incompleted"
                                                }).ToList();

            return new PagedResultDto<ResponsesModel>(total, result);
        }
    }
}
