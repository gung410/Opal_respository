using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Communication.Business.HttpClients
{
    public interface IFirebaseCloudMessageHttpClient
    {
        Task SendNotificationAsync(dynamic message);
    }
}
