using System.Collections.Generic;

namespace cxOrganization.Adapter.JobChannel
{
    public interface IJobChannelAdapter
    {
        List<dynamic> GetCvCompletenessStatuses(List<string> objectIds);
    }
}
