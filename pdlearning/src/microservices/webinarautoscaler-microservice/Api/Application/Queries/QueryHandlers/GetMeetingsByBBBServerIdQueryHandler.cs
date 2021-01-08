using System.Collections.Generic;
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
    public class GetMeetingsByBBBServerIdQueryHandler : BaseQueryHandler<GetMeetingsByBBBServerIdQuery, List<MeetingInfoModel>>
    {
        private readonly IRepository<MeetingInfo> _meetingInfoRepository;

        public GetMeetingsByBBBServerIdQueryHandler(
            IRepository<MeetingInfo> meetingInfoRepository,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager)
        {
            _meetingInfoRepository = meetingInfoRepository;
        }

        protected override Task<List<MeetingInfoModel>> HandleAsync(GetMeetingsByBBBServerIdQuery query, CancellationToken cancellationToken)
        {
            return _meetingInfoRepository
                .GetAll()
                .Where(p => p.BBBServerId == query.BBBServerId)
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
