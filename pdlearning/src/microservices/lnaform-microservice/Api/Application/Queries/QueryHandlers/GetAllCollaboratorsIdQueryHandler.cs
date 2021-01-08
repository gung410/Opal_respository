using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.LnaForm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Queries
{
    public class GetAllCollaboratorsIdQueryHandler : BaseThunderQueryHandler<GetAllCollaboratorsIdQuery, List<Guid>>
    {
        private readonly IRepository<AccessRight> _accessRightRepository;

        public GetAllCollaboratorsIdQueryHandler(IRepository<AccessRight> accessRightRepository)
        {
            _accessRightRepository = accessRightRepository;
        }

        protected override async Task<List<Guid>> HandleAsync(GetAllCollaboratorsIdQuery query, CancellationToken cancellationToken)
        {
            var collaboratorIds = await _accessRightRepository
                .GetAll()
                .Where(c => c.ObjectId == query.Request.OriginalObjectId)
                .Select(p => p.UserId)
                .ToListAsync(cancellationToken);

            return collaboratorIds;
        }
    }
}
