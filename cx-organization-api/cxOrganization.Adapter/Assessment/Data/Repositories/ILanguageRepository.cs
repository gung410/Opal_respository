using System.Collections.Generic;
using cxOrganization.Adapter.Assessment.Data.Entities;

namespace cxOrganization.Adapter.Assessment.Data.Repositories
{
    public interface ILanguageRepository
    {
        List<LanguageEntity> GetLanguages();
    }
}
