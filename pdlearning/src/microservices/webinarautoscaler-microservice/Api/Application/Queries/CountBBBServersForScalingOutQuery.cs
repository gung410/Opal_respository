using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.WebinarAutoscaler.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarAutoscaler.Application.Queries
{
    public class CountBBBServersForScalingOutQuery : BaseThunderQuery<int>
    {
    }
}