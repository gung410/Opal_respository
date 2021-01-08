using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarAutoscaler.Application.Models;
using Microservice.WebinarAutoscaler.Domain.Entities;
using Microservice.WebinarAutoscaler.Domain.Enums;
using Microservice.WebinarAutoscaler.Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Microservice.WebinarAutoscaler.Application.Queries.QueryHandlers
{
    public class GetBBBServersQueryHandler : BaseQueryHandler<GetBBBServersQuery, List<BBBServerModel>>
    {
        private readonly IRepository<BBBServer> _bbbServerRepository;
        private readonly IRepository<MeetingInfo> _meetingInfoRepository;

        public GetBBBServersQueryHandler(
            IRepository<BBBServer> bbbServerRepository,
            IRepository<MeetingInfo> meetingInfoRepository,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager)
        {
            _bbbServerRepository = bbbServerRepository;
            _meetingInfoRepository = meetingInfoRepository;
        }

        protected override Task<List<BBBServerModel>> HandleAsync(GetBBBServersQuery query, CancellationToken cancellationToken)
        {
            var now = Clock.Now;
            return _bbbServerRepository
                    .GetAll()
                    .Where(p => p.Status == BBBServerStatus.Ready)
                    .Select(p => new BBBServerModel
                    {
                        Id = p.Id,
                        PrivateIp = p.PrivateIp,
                        Status = p.Status,
                        IsProtection = p.IsProtection,
                        InstanceId = p.InstanceId,
                        RuleArn = p.RuleArn,
                        TargetGroupArn = p.TargetGroupArn,

                        // Count meetings that are still running.
                        ParticipantCount = _meetingInfoRepository
                                            .GetAll()
                                            .Where(x => x.BBBServerId == p.Id)
                                            .Where(x => x.EndTime >= now || (x.EndTime < now && x.IsLive))
                                            .Sum(p => p.ParticipantCount)
                    })
                    .OrderByDescending(x => x.ParticipantCount)
                    .Distinct()
                    .ToListAsync(cancellationToken);
        }
    }
}
