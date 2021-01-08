using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Settings;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetMaxMinutesCanJoinWebinarEarlyQueryHandler : BaseQueryHandler<GetMaxMinutesCanJoinWebinarEarlyQuery, int>
    {
        private readonly OpalSettingsOption _opalSettingsOption;

        public GetMaxMinutesCanJoinWebinarEarlyQueryHandler(
            IOptions<OpalSettingsOption> opalSettingsOption,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _opalSettingsOption = opalSettingsOption.Value;
        }

        protected override Task<int> HandleAsync(GetMaxMinutesCanJoinWebinarEarlyQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult(_opalSettingsOption.MaxMinutesCanJoinWebinarEarly);
        }
    }
}
