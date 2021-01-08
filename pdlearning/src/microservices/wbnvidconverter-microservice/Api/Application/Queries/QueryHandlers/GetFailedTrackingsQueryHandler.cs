using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarVideoConverter.Domain.Entities;
using Microservice.WebinarVideoConverter.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.WebinarVideoConverter.Application.Queries.QueryHandlers
{
    public class GetFailedTrackingsQueryHandler : BaseQueryHandler<GetFailedTrackingsQuery, List<ConvertingTracking>>
    {
        private readonly IRepository<ConvertingTracking> _convertingTrackingRepo;

        public GetFailedTrackingsQueryHandler(
            IRepository<ConvertingTracking> convertingTrackingRepo,
            IUnitOfWorkManager unitOfWorkManager)
            : base(unitOfWorkManager)
        {
            _convertingTrackingRepo = convertingTrackingRepo;
        }

        protected override Task<List<ConvertingTracking>> HandleAsync(GetFailedTrackingsQuery query, CancellationToken cancellationToken)
        {
            return _convertingTrackingRepo
                .GetAll()
                .Where(c => c.Status == ConvertStatus.Failed)
                .ToListAsync(cancellationToken);
        }
    }
}
