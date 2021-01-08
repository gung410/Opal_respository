using System.Collections.Generic;


namespace cxOrganization.Adapter.JobChannel
{
    public class UnusedJobChannelAdapter : IJobChannelAdapter
    {

        public List<dynamic> GetCvCompletenessStatuses(List<string> objectIds)
        {
            return new List<dynamic>();
        }
    }
}