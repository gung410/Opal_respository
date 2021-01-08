using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Content.Application.Models;
using Microservice.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Queries.QueryHandlers
{
    public class GetExpiryInfoOfDigitalContentsQueryHandler : BaseThunderQueryHandler<GetExpiryInfoOfDigitalContentsQuery, DigitalContentExpiryInfoModel[]>
    {
        private readonly IRepository<DigitalContent> _dcRepository;

        public GetExpiryInfoOfDigitalContentsQueryHandler(IRepository<DigitalContent> digitalContentRepository)
        {
            _dcRepository = digitalContentRepository;
        }

        protected override async Task<DigitalContentExpiryInfoModel[]> HandleAsync(GetExpiryInfoOfDigitalContentsQuery query, CancellationToken cancellationToken)
        {
            if (query.ListDigitalContentId == null || query.ListDigitalContentId.Length == 0)
            {
                return new DigitalContentExpiryInfoModel[0];
            }

            var result = await _dcRepository
                .GetAll()
                .Where(p => query.ListDigitalContentId.Contains(p.Id))
                .Select(p => new DigitalContentExpiryInfoModel
                {
                    Id = p.Id,
                    ExpiredDate = p.ExpiredDate
                })
                .ToArrayAsync(cancellationToken);

            return result;
        }
    }
}
