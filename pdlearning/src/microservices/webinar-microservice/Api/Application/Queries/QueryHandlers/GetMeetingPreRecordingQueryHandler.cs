using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Webinar.Application.Models;
using Microservice.Webinar.Application.Services.BigBlueButton;
using Microservice.Webinar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Queries.QueryHandlers
{
    public class GetMeetingPreRecordingQueryHandler : BaseThunderQueryHandler<GetMeetingPreRecordingQuery, List<ResultGetMeetingPreRecordingModel>>
    {
        private readonly IRepository<MeetingInfo> _meetingInfoRepository;
        private readonly BigBlueButtonServerOptions _bigBlueButtonServer;

        public GetMeetingPreRecordingQueryHandler(IRepository<MeetingInfo> meetingInfoRepository, IOptions<BigBlueButtonServerOptions> bigBlueButtonServer)
        {
            _meetingInfoRepository = meetingInfoRepository;
            _bigBlueButtonServer = bigBlueButtonServer.Value;
        }

        protected override Task<List<ResultGetMeetingPreRecordingModel>> HandleAsync(GetMeetingPreRecordingQuery query, CancellationToken cancellationToken)
        {
            var now = Clock.Now;
            return _meetingInfoRepository
                .GetAll()
                .Where(p => query.MeetingIds.Any(x => x.Equals(p.Id)))
                .Where(p => p.StartTime <= now && p.EndTime >= now)
                .Where(p => !string.IsNullOrEmpty(p.PreRecordPath))
                .Select(p => new ResultGetMeetingPreRecordingModel
                {
                    MeetingId = p.Id,
                    PreRecordUrl = $"{_bigBlueButtonServer.CloudfrontUrl}{(p.PreRecordPath.StartsWith("/") ? string.Empty : "/")}{p.PreRecordPath}"
                })
                .ToListAsync(cancellationToken);
        }
    }
}
