using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarVideoConverter.Application.Models;
using Microservice.WebinarVideoConverter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.WebinarVideoConverter.Application.Queries
{
    public class GetConvertingTrackingsByStatusQueryHandler : BaseQueryHandler<GetConvertingTrackingsByStatusQuery, List<ConvertingTrackingModel>>
    {
        private readonly IRepository<ConvertingTracking> _convertTrackingRepository;

        public GetConvertingTrackingsByStatusQueryHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<ConvertingTracking> convertTrackingRepository) : base(unitOfWorkManager)
        {
            _convertTrackingRepository = convertTrackingRepository;
        }

        protected override Task<List<ConvertingTrackingModel>> HandleAsync(GetConvertingTrackingsByStatusQuery query, CancellationToken cancellationToken)
        {
            return _convertTrackingRepository
                .GetAll()
                .Where(p => p.Status == query.Status)
                .Select(p => new ConvertingTrackingModel
                {
                    Id = p.Id,
                    MeetingId = p.MeetingId,
                    InternalMeetingId = p.InternalMeetingId,
                    Status = p.Status
                })
                .ToListAsync(cancellationToken);
        }
    }
}
