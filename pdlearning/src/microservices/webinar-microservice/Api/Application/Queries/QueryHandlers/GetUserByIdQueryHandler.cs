using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Webinar.Application.Models;
using Microservice.Webinar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Queries.QueryHandlers
{
    public class GetUserByIdQueryHandler : BaseThunderQueryHandler<GetUserByIdQuery, UserModel>
    {
        private readonly IRepository<WebinarUser> _userRepository;

        public GetUserByIdQueryHandler(IRepository<WebinarUser> userRepository)
        {
            _userRepository = userRepository;
        }

        protected override Task<UserModel> HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            return _userRepository.GetAll()
                .Where(p => p.Id == query.Id)
                .Select(p => new UserModel
                {
                    Id = p.Id,
                    UserId = p.OriginalUserId,
                    FirstName = p.FirstName,
                    LastName = p.LastName
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
