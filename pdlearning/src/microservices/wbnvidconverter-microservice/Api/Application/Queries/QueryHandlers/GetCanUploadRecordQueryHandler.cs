using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarVideoConverter.Application.Models;
using Microservice.WebinarVideoConverter.Domain.Entities;
using Microservice.WebinarVideoConverter.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.WebinarVideoConverter.Application.Queries.QueryHandlers
{
    public class GetCanUploadRecordQueryHandler : BaseQueryHandler<GetCanUploadRecordQuery, List<ConvertingTrackingModel>>
    {
        private readonly IRepository<ConvertingTracking> _convertingTrackingRepository;

        public GetCanUploadRecordQueryHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<ConvertingTracking> convertingTrackingRepository) : base(unitOfWorkManager)
        {
            _convertingTrackingRepository = convertingTrackingRepository;
        }

        protected override async Task<List<ConvertingTrackingModel>> HandleAsync(GetCanUploadRecordQuery query, CancellationToken cancellationToken)
        {
            var result = await _convertingTrackingRepository
                .GetAll()
                .Where(x => x.Status == ConvertStatus.Converted)
                .Take(query.MaxConcurrentUploads)
                .ToListAsync(cancellationToken);

            if (result == null || !result.Any())
            {
                return null;
            }

            return result.Select(x => new ConvertingTrackingModel
            {
                Id = x.Id,
                MeetingId = x.MeetingId,
                InternalMeetingId = x.InternalMeetingId,
                Status = x.Status
            }).ToList();
        }
    }
}
