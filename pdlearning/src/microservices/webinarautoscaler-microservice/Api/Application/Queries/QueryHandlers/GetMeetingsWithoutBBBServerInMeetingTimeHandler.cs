using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarAutoscaler.Application.Models;
using Microservice.WebinarAutoscaler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Microservice.WebinarAutoscaler.Application.Queries.QueryHandlers
{
    public class GetMeetingsWithoutBBBServerInMeetingTimeHandler : BaseQueryHandler<GetMeetingsWithoutBBBServerInMeetingTime, List<MeetingInfoModel>>
    {
        private readonly IRepository<MeetingInfo> _meetingInfoRepository;

        public GetMeetingsWithoutBBBServerInMeetingTimeHandler(
            IRepository<MeetingInfo> meetingInfoRepository,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager)
        {
            _meetingInfoRepository = meetingInfoRepository;
        }

        protected override Task<List<MeetingInfoModel>> HandleAsync(GetMeetingsWithoutBBBServerInMeetingTime query, CancellationToken cancellationToken)
        {
            var nowStartTime = Clock.Now.AddMinutes(30);
            var nowEndTime = Clock.Now.AddMinutes(-30);
            return _meetingInfoRepository
                .GetAll()
                .Where(p => p.BBBServerId == null && nowStartTime > p.StartTime && nowEndTime <= p.EndTime)
                .OrderByDescending(p => p.ParticipantCount)
                .Select(p => new MeetingInfoModel
                {
                    Id = p.Id,
                    BBBServerId = p.BBBServerId,
                    Title = p.Title,
                    IsCanceled = p.IsCanceled,
                    EndTime = p.EndTime,
                    StartTime = p.StartTime,
                    ParticipantCount = p.ParticipantCount,
                    PreRecordId = p.PreRecordId,
                    PreRecordPath = p.PreRecordPath,
                    IsLive = p.IsLive
                })
                .ToListAsync(cancellationToken);
        }
    }
}
