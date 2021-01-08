using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarAutoscaler.Application.Models;
using Microservice.WebinarAutoscaler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.WebinarAutoscaler.Application.Queries.QueryHandlers
{
    public class GetBBBServerByIdQueryHandler : BaseQueryHandler<GetBBBServerByIdQuery, BBBServerModel>
    {
        private readonly IRepository<BBBServer> _bbbServerRepository;

        public GetBBBServerByIdQueryHandler(
            IRepository<BBBServer> bbbServerRepository,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager)
        {
            _bbbServerRepository = bbbServerRepository;
        }

        protected override Task<BBBServerModel> HandleAsync(GetBBBServerByIdQuery query, CancellationToken cancellationToken)
        {
            return _bbbServerRepository
                .GetAll()
                .Where(bbbServer => bbbServer.Id.Equals(query.BBBServerId))
                .Select(p => new BBBServerModel
                {
                    Id = p.Id,
                    PrivateIp = p.PrivateIp,
                    Status = p.Status,
                    IsProtection = p.IsProtection,
                    InstanceId = p.InstanceId,
                    RuleArn = p.RuleArn,
                    TargetGroupArn = p.TargetGroupArn
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
