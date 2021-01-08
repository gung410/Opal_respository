using System.Collections.Generic;
using cxPlatform.Core;

namespace cxOrganization.Domain.Services.ExportService
{
    public interface IExportService<T> where T : class
    {
        byte[] ExportDataToBytes(IList<T> source, ExportOption exportOption, IWorkContext currentWorkContext = null);
        byte[] ExportDataToBytes(T source, ExportOption exportOption, IWorkContext currentWorkContext = null);

        byte[] ExportDataToBytes(IDictionary<string, List<T>> source, ExportOption exportOption,
            IWorkContext currentWorkContext = null);

    }
}
