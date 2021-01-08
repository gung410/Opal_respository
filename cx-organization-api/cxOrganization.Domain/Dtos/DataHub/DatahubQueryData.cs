using cxPlatform.Core.DatahubLog;
using System.Collections.Generic;

namespace cxOrganization.Domain.Dtos.DataHub
{
    public class DataHubQueryData
    {
        public List<GenericLogEventMessage> EventMany { get; set; }

    }
}