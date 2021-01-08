using cxOrganization.Domain.DomainEnums;
using cxOrganization.Domain.Settings;
using System.Collections.Generic;
using System.IO;

namespace cxOrganization.Domain.Services
{
    public interface IDataFileReader
    {
        FileType FileType { get; }
        List<T> ReadDataFromStream<T>(Stream stream, PropertyMappingSetting mappingSetting) where T : new();
    }
}
