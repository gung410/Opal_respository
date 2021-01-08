using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarAutoscaler.Domain.Entities;
using Microservice.WebinarAutoscaler.Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.WebinarAutoscaler.Application.Queries.QueryHandlers
{
    public class GetBBBServerIdsForScalingInQueryHandler : BaseQueryHandler<GetBBBServerIdsForScalingInQuery, List<Guid>>
    {
        private readonly IRepository<BBBServer> _bbbServerRepository;
        private readonly IRepository<MeetingInfo> _meetingInfoRepository;

        public GetBBBServerIdsForScalingInQueryHandler(
            IRepository<BBBServer> bbbServerRepository,
            IRepository<MeetingInfo> meetingInfoRepository,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager)
        {
            _bbbServerRepository = bbbServerRepository;
            _meetingInfoRepository = meetingInfoRepository;
        }

        protected override Task<List<Guid>> HandleAsync(GetBBBServerIdsForScalingInQuery query, CancellationToken cancellationToken)
        {
            // Get bbb server ids with unprotected and that have all owned meeting ended(after end time 30m or canceled )
            return _bbbServerRepository
                 .GetAll()
                 .GetUnprotectedBBBServersHaveEndedMeetings(_meetingInfoRepository)
                 .GetBBBServerIdsWithAllMeetingEnded(_meetingInfoRepository)
                 .ToListAsync(cancellationToken);
        }
    }
}
