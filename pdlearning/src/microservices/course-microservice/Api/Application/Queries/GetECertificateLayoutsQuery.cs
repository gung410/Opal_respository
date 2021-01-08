using System.Collections.Generic;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetECertificateLayoutsQuery : BaseThunderQuery<IEnumerable<ECertificateLayoutModel>>
    {
    }
}
