using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cxOrganization.Adapter.Assessment.Data.Entities;
using cxOrganization.Adapter.Shared.Entity;
using cxPlatform.Core;
using Microsoft.EntityFrameworkCore;

namespace cxOrganization.Adapter.Assessment.Data.Repositories
{
    /// <summary>
    /// ActivityRepository
    /// </summary>
    public class StatusTypeRepository : RepositoryBase<StatusTypeEntity>, IStatusTypeRepository
    {
        const int MaximumRecordsReturn = 5000;
        private readonly AssessmentConfigContext _assessmentConfigContext;

        /// <summary>
        /// Status type repository
        /// </summary>
        /// <param name="assessmentConfigContext"></param>
        public StatusTypeRepository(AssessmentConfigContext assessmentConfigContext)
            : base(assessmentConfigContext)
        {
            _assessmentConfigContext = assessmentConfigContext;
        }

        public List<StatusTypeEntity> GetStatusTypes()
        {
            return GetAllAsNoTracking().ToList();
        }

        public Task<List<int>> GetStatusTypeIdsByCodeNames(List<string> statusTypeCodeNames)
        {
            return GetAllAsNoTracking()
                .Where(s => statusTypeCodeNames != null && statusTypeCodeNames.Contains(s.CodeName))
                .Select(s => s.StatusTypeID)
                .ToListAsync();
        }
    }
}