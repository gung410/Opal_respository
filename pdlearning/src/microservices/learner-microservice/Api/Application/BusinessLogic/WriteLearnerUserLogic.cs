using System.Linq;
using System.Threading.Tasks;
using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microservice.Learner.Application.RequestDtos;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.BusinessLogic
{
    public class WriteLearnerUserLogic : IWriteLearnerUserLogic
    {
        private readonly IRepository<LearnerUser> _learnerUserRepository;

        public WriteLearnerUserLogic(IRepository<LearnerUser> learnerUserRepository)
        {
            _learnerUserRepository = learnerUserRepository;
        }

        public async Task SaveLearnerUser(SaveLearnerUserRequestDto request)
        {
            var existedUser = await _learnerUserRepository
                .GetAll()
                .Where(u => u.OriginalUserId == request.Identity.Id)
                .Where(u => u.Id == request.Identity.ExtId)
                .FirstOrDefaultAsync();

            var toSaveUser = existedUser == null
                ? new LearnerUser()
                {
                    Id = request.Identity.ExtId.Value,
                    OriginalUserId = request.Identity.Id
                }
                : existedUser;

            toSaveUser.DepartmentId = request.DepartmentId;
            toSaveUser.Email = request.EmailAddress;
            toSaveUser.FirstName = request.FirstName;
            toSaveUser.LastName = request.LastName;
            toSaveUser.Status = request.EntityStatus.Status;
            toSaveUser.AccountType = request.EntityStatus.ExternallyMastered ? LearnerUserAccountType.Internal : LearnerUserAccountType.External;
            toSaveUser.PrimaryApprovingOfficerId = request.PrimaryApprovingOfficerId;
            toSaveUser.AlternativeApprovingOfficerId = request.AlternativeApprovingOfficerId;

            if (existedUser == null)
            {
                await _learnerUserRepository.InsertAsync(toSaveUser);
            }
            else
            {
                await _learnerUserRepository.UpdateAsync(toSaveUser);
            }
        }
    }
}
