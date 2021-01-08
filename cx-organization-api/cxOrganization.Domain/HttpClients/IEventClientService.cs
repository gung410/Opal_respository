using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cxOrganization.Domain.HttpClients
{
    public interface IEventClientService
    {
        Task SendEvent(dynamic eventData);
    }
}
