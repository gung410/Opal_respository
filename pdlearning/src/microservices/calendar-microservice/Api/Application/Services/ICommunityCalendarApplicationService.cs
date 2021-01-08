using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Calendar.Application.Services
{
    public interface ICommunityCalendarApplicationService
    {
        Task<List<CommunityEventModel>> GetCommunityEvents(GetCommunityEventRequest request, Guid communityId);

        Task<List<CommunityEventModel>> GetCommunityEventsByUser(GetMyCommunityEventRequest request, Guid userId);

        Task<CommunityEventModel> GetCommunityEvent(Guid id);

        Task<CommunityEventModel> CreateCommunityEvent(CreateCommunityEventRequest request, Guid userId);

        Task<CommunityEventModel> UpdateCommunityEvent(UpdateCommunityEventRequest request, Guid userId);

        Task DeleteCommunityEvent(Guid eventId, Guid userId);

        Task<PagedResultDto<CommunityEventModel>> GetCommunityEventsByCommunityId(Guid userId, GetCommunityEventsByCommunityIdRequest request);

        Task<CommunityEventModel> CreateWebinarCommunityEvent(CreateCommunityEventRequest request, Guid userId);

        Task<CommunityEventModel> UpdateWebinarCommunityEvent(UpdateCommunityEventRequest request, Guid userId);

        Task DeleteWebinarCommunityEvent(Guid eventId, Guid userId);
    }
}
