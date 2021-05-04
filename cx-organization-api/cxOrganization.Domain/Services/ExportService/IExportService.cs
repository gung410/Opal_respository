using System.Collections.Generic;
using cxOrganization.Domain.AdvancedWorkContext;
using cxPlatform.Core;

namespace cxOrganization.Domain.Services.ExportService
{
    public interface IExportService<T> where T : class
    {
        byte[] ExportDataToBytes(IList<T> source, ExportOption exportOption, IAdvancedWorkContext currentWorkContext = null);
        byte[] ExportDataToBytes(T source, ExportOption exportOption, IAdvancedWorkContext currentWorkContext = null);

        byte[] ExportDataToBytes(IDictionary<string, List<T>> source, ExportOption exportOption,
            IAdvancedWorkContext currentWorkContext = null);

    }
}
