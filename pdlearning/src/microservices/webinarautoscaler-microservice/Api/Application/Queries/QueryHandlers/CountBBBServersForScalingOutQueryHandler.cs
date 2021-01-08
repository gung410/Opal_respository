using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarAutoscaler.Application.Models;
using Microservice.WebinarAutoscaler.Domain.Entities;
using Microservice.WebinarAutoscaler.Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Microservice.WebinarAutoscaler.Application.Queries.QueryHandlers
{
    public class CountBBBServersForScalingOutQueryHandler : BaseQueryHandler<CountBBBServersForScalingOutQuery, int>
    {
        private readonly IRepository<BBBServer> _bbbServerRepository;
        private readonly IRepository<MeetingInfo> _meetingInfoRepository;

        public CountBBBServersForScalingOutQueryHandler(
            IRepository<BBBServer> bbbServerRepository,
            IRepository<MeetingInfo> meetingInfoRepository,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager)
        {
            _bbbServerRepository = bbbServerRepository;
            _meetingInfoRepository = meetingInfoRepository;
        }

        protected override Task<int> HandleAsync(CountBBBServersForScalingOutQuery query, CancellationToken cancellationToken)
        {
            // Get BBB server with no owned meeting.
            return _bbbServerRepository
                  .GetAll()
                  .GetBBBServersHaveNoMeetingsOwned(_meetingInfoRepository)
                  .CountAsync(cancellationToken);
        }
    }
}
