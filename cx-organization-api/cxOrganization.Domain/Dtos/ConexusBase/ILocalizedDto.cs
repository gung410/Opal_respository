using System.Collections.Generic;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Client.ConexusBase
{
    public interface ILocalizedDto
    {
        List<LocalizedDataDto> LocalizedData { get; set; }
    }
}