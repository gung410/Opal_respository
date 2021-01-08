using cxOrganization.Domain.Entities;
using System.Collections.Generic;

namespace cxOrganization.Domain.Repositories
{
    public interface ILanguageRepository
    {
        List<LanguageEntity> GetLanguages();
        LanguageEntity GetById(params object[] Id);

    }
}
