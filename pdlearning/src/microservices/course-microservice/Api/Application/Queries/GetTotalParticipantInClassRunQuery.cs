using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetTotalParticipantInClassRunQuery : BaseThunderQuery<IEnumerable<TotalParticipantInClassRunModel>>
    {
        private List<Guid> _classRunIds;

        public List<Guid> ClassRunIds
        {
            get => _classRunIds;
            set => _classRunIds = value.Distinct().ToList();
        }
    }
}
