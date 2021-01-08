using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Queries.QueryHandlers
{
    public class GetAllCollaboratorsIdQueryHandler : BaseThunderQueryHandler<GetAllCollaboratorsIdQuery, List<Guid>>
    {
        private readonly IRepository<AccessRight> _accessRightRepository;

        public GetAllCollaboratorsIdQueryHandler(IRepository<AccessRight> accessRightRepository)
        {
            _accessRightRepository = accessRightRepository;
        }

        protected override Task<List<Guid>> HandleAsync(GetAllCollaboratorsIdQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _accessRightRepository
                .GetAll()
                .Where(c => c.ObjectId == query.OriginalObjectId)
                .Select(p => p.UserId)
                .ToListAsync(cancellationToken);

            return dbQuery;
        }
    }
}
