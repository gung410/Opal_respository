using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetBlockoutDateByIdQueryHandler : BaseQueryHandler<GetBlockoutDateByIdQuery, BlockoutDateModel>
    {
        private readonly IReadOnlyRepository<BlockoutDate> _readBlockoutDateRepository;

        public GetBlockoutDateByIdQueryHandler(
            IReadOnlyRepository<BlockoutDate> readBlockoutDateRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readBlockoutDateRepository = readBlockoutDateRepository;
        }

        protected override async Task<BlockoutDateModel> HandleAsync(GetBlockoutDateByIdQuery query, CancellationToken cancellationToken)
        {
            var blockoutDate = await _readBlockoutDateRepository.GetAsync(query.Id);

            return new BlockoutDateModel(blockoutDate);
        }
    }
}
