using System;
using System.Collections.Generic;
using cxOrganization.Adapter.Assessment.Data.Entities;
using cxPlatform.Core;

namespace cxOrganization.Adapter.Assessment.Data.Repositories
{
    /// <summary>
    /// IEventRepository
    /// </summary>
    public interface IAlternativeRepository : IRepository<AlternativeEntity>
    {
        List<AlternativeEntity> GetAlternatives(
                List<int> alternativeIds = null,
                List<int> scaleIds = null,
                List<string> extIds = null,
                List<int> activityIds = null,
                DateTime? createdAfter = null,
                DateTime? createdBefore = null);
    }
}
