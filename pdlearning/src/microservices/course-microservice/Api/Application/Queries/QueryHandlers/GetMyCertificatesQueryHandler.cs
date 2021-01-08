using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetMyCertificatesQueryHandler : BaseQueryHandler<GetMyCertificatesQuery, PagedResultDto<RegistrationModel>>
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;

        public GetMyCertificatesQueryHandler(
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<Registration> readRegistrationRepository) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readRegistrationRepository = readRegistrationRepository;
        }

        protected override async Task<PagedResultDto<RegistrationModel>> HandleAsync(GetMyCertificatesQuery query, CancellationToken cancellationToken)
        {
            var registrationQuery = _readRegistrationRepository.GetAll()
                .Where(p => p.UserId == CurrentUserId && p.ECertificate != null);

            var totalCount = await registrationQuery.CountAsync(cancellationToken);

            if (query.PageInfo != null && query.PageInfo.MaxResultCount == 0)
            {
                return new PagedResultDto<RegistrationModel>(totalCount);
            }

            var userRegistrationECertificates = await ApplyPaging(registrationQuery.OrderByDescending(p => p.LearningCompletedDate), query.PageInfo).ToListAsync(cancellationToken);
            return new PagedResultDto<RegistrationModel>(
                totalCount,
                userRegistrationECertificates
                    .Select(p => new RegistrationModel(p))
                    .ToList());
        }
    }
}
