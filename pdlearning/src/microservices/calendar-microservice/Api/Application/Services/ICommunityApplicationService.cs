using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;

namespace Microservice.Calendar.Application.Services
{
    public interface ICommunityApplicationService
    {
        Task<List<CommunityModel>> GetCommunityHierarchyTree(Guid userId);

        Task<List<CommunityModel>> GetOwnCommunities(Guid userId);
    }
}
