using System.Linq;
using System.Threading.Tasks;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Course.Application.BusinessLogics
{
    public class SaveUserLogic : BaseBusinessLogic
    {
        private readonly IRepository<CourseUser> _userRepository;
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly GetAggregatedRegistrationSharedQuery _aggregatedRegistrationSharedQuery;
        private readonly RegistrationCudLogic _registrationCudLogic;

        public SaveUserLogic(
            IRepository<CourseUser> userRepository,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            RegistrationCudLogic registrationCudLogic,
            IUserContext userContext,
            GetAggregatedRegistrationSharedQuery aggregatedRegistrationSharedQuery) : base(userContext)
        {
            _userRepository = userRepository;
            _readRegistrationRepository = readRegistrationRepository;
            _registrationCudLogic = registrationCudLogic;
            _aggregatedRegistrationSharedQuery = aggregatedRegistrationSharedQuery;
        }

        public async Task SaveCourseUser(SaveCourseUserRequestDto request)
        {
            if (request.Identity.ExtId == null)
            {
                return;
            }

            var existedUser = await _userRepository.FirstOrDefaultAsync(u => u.OriginalUserId == request.Identity.Id && u.Id == request.Identity.ExtId);

            var toSaveUser = existedUser == null
                ? new CourseUser()
                {
                    Id = request.Identity.ExtId.Value,
                    OriginalUserId = request.Identity.Id
                }
                : existedUser;

            toSaveUser.DepartmentId = request.DepartmentId;
            toSaveUser.Email = request.EmailAddress;
            toSaveUser.FirstName = request.FirstName;
            toSaveUser.LastName = request.LastName;
            toSaveUser.PhoneNumber = request.PhoneNumber;
            toSaveUser.Status = request.EntityStatus.Status;
            toSaveUser.AccountType = request.EntityStatus.ExternallyMastered ? CourseUserAccountType.Internal : CourseUserAccountType.External;
            toSaveUser.PrimaryApprovingOfficerId = request.PrimaryApprovingOfficerId;
            toSaveUser.AlternativeApprovingOfficerId = request.AlternativeApprovingOfficerId;
            toSaveUser.Track = request.TrackIds;
            toSaveUser.TeachingLevel = request.TeachingLevelIds;
            toSaveUser.TeachingCourseOfStudy = request.TeachingCourseOfStudyIds;
            toSaveUser.TeachingSubject = request.TeachingSubjectIds;
            toSaveUser.CocurricularActivity = request.CocurricularActivityIds;
            toSaveUser.DevelopmentalRole = request.DevelopmentalRoleIds;
            toSaveUser.ServiceScheme = request.ServiceSchemeIds;
            toSaveUser.JobFamily = request.JobFamilyIds;
            toSaveUser.EasSubstantiveGradeBanding = request.EasSubstantiveGradeBandingIds;
            toSaveUser.SystemRoles = request.SystemRoles;
            toSaveUser.LearningFramework = request.LearningFrameworks;
            toSaveUser.Designation = request.Designations;

            if (existedUser == null)
            {
                await _userRepository.InsertAsync(toSaveUser);
            }
            else
            {
                await _userRepository.UpdateAsync(toSaveUser);
            }

            var aggregatedRegistrations = await _aggregatedRegistrationSharedQuery.FullByQuery(
                _readRegistrationRepository.GetAll().Where(p => p.UserId == request.Identity.ExtId && p.LearningStatus == LearningStatus.NotStarted));

            foreach (var item in aggregatedRegistrations)
            {
                if (item.Registration.ApprovingOfficer != request.PrimaryApprovingOfficerId || item.Registration.AlternativeApprovingOfficer != request.AlternativeApprovingOfficerId)
                {
                    item.Registration.ApprovingOfficer = request.PrimaryApprovingOfficerId;
                    item.Registration.AlternativeApprovingOfficer = request.AlternativeApprovingOfficerId;
                    await _registrationCudLogic.Update(item);
                }
            }
        }

        public async Task SaveCourseUserEntityStatus(SaveCourseUserRequestDto request)
        {
            if (request.Identity.ExtId == null)
            {
                return;
            }

            var existedUser = await _userRepository.FirstOrDefaultAsync(u => u.OriginalUserId == request.Identity.Id && u.Id == request.Identity.ExtId);

            if (existedUser != null)
            {
                existedUser.Status = request.EntityStatus.Status;
                await _userRepository.UpdateAsync(existedUser);
            }
        }
    }
}
