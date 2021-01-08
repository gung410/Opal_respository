using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice.Analytics.Application.Consumers.CSL.Messages
{
    public class CSLFileRemovedMessage
    {
        public int? Id { get; set; }

        public string ObjectModel { get; set; }
    }
}
