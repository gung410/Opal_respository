using System.Collections.Generic;
using System.Linq;
using cxOrganization.Adapter.Assessment.Data.Entities;
using cxPlatform.Core;

namespace cxOrganization.Adapter.Assessment.Data.Repositories
{
    public class LanguageRepository : RepositoryBase<LanguageEntity>, ILanguageRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageRepository"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public LanguageRepository(AssessmentConfigContext assessmentConfigContext)
            : base(assessmentConfigContext)
        {
        }

        public List<LanguageEntity> GetLanguages()
        {
            return GetAllAsNoTracking().ToList();
        }
    }
}
