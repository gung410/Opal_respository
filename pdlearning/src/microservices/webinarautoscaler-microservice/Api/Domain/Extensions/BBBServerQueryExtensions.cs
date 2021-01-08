using System;
using System.Linq;
using Microservice.WebinarAutoscaler.Application.Models;
using Microservice.WebinarAutoscaler.Domain.Entities;
using Microservice.WebinarAutoscaler.Domain.Enums;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.WebinarAutoscaler.Domain.Extensions
{
    public static class BBBServerQueryExtensions
    {
        public static IQueryable<BBBServer> GetUnprotectedBBBServersHaveEndedMeetings<TBBBServer>(this TBBBServer @bbbServers, IRepository<MeetingInfo> meetingInfoRepository) where TBBBServer : IQueryable<BBBServer>
        {
            var now = Clock.Now.AddMinutes(-30);
            return @bbbServers
                  .Where(x => x.Status == BBBServerStatus.Ready)
                  .Where(x => x.IsProtection == false)
                  .GroupJoinMeeting(meetingInfoRepository)
                  .Where(p => p.Meeting != null && ((now > p.Meeting.EndTime && !p.Meeting.IsLive) || p.Meeting.IsCanceled))
                  .Select(x => x.BBBServer);
        }

        public static IQueryable<BBBServer> GetBBBServersHaveNoMeetingsOwned<TBBBServer>(this TBBBServer @bbbServers, IRepository<MeetingInfo> meetingInfoRepository) where TBBBServer : IQueryable<BBBServer>
        {
            return @bbbServers
                  .GroupJoinMeeting(meetingInfoRepository)
                  .Where(p => p.Meeting == null)
                  .Distinct()
                  .Select(x => x.BBBServer);
        }

        public static IQueryable<BBBServerMeetingModel> GroupJoinMeeting<TBBBServer>(this TBBBServer @bbbServers, IRepository<MeetingInfo> meetingInfoRepository) where TBBBServer : IQueryable<BBBServer>
        {
            return @bbbServers
                  .GroupJoin(
                            meetingInfoRepository.GetAll(),
                            p => p.Id,
                            p => p.BBBServerId,
                            (bbbServers, meetings) => new { bbbServers, meetings })
                  .SelectMany(
                            p => p.meetings.DefaultIfEmpty(),
                            (collection, meeting) => new { collection.bbbServers, meeting })
                  .Select(x =>
                  new BBBServerMeetingModel
                  {
                      BBBServer = x.bbbServers,
                      Meeting = x.meeting
                  });
        }

        public static IQueryable<Guid> GetBBBServerIdsWithAllMeetingEnded<TBBBServer>(this TBBBServer @bbbServers, IRepository<MeetingInfo> meetingInfoRepository) where TBBBServer : IQueryable<BBBServer>
        {
            return @bbbServers
                    .GroupBy(p => p.Id, p => p.Id, (id, gr) => new
                        {
                            BBBServerId = id,
                            MeetingCount = gr.Count()
                        })
                    .Where(p =>
                        p.MeetingCount > 0 &&
                        p.MeetingCount == meetingInfoRepository
                                            .GetAll()
                                            .Where(x => x.BBBServerId == p.BBBServerId)
                                            .Count())
                    .Select(x => x.BBBServerId);
        }
    }
}
