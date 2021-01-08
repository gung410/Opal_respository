using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class SaveCourseCriteriaCommandHandler : BaseCommandHandler<SaveCourseCriteriaCommand>
    {
        private readonly IReadOnlyRepository<CourseCriteria> _readCourseCriteriaRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly CourseCriteriaCudLogic _courseCriteriaCudLogic;

        public SaveCourseCriteriaCommandHandler(
            IReadOnlyRepository<CourseCriteria> readCourseCriteriaRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            CourseCriteriaCudLogic courseCriteriaCudLogic,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseCriteriaRepository = readCourseCriteriaRepository;
            _readCourseRepository = readCourseRepository;
            _courseCriteriaCudLogic = courseCriteriaCudLogic;
        }

        protected override async Task HandleAsync(SaveCourseCriteriaCommand command, CancellationToken cancellationToken)
        {
            var courseCriteria = await _readCourseCriteriaRepository.FirstOrDefaultAsync(p => p.Id == command.Id);
            var existedCourse = await _readCourseRepository.GetAsync(command.Id);

            EnsureBusinessLogicValid(existedCourse.ValidateNotArchived());
            EnsureValidPermission(
                existedCourse.HasSaveCourseCriteriaPermission(
                    CurrentUserId,
                    CurrentUserRoles,
                    _readCourseRepository.GetHasAdminRightChecker(existedCourse, AccessControlContext)));

            if (courseCriteria == null)
            {
                await CreateNew(command, cancellationToken);
            }
            else
            {
                await Update(command, courseCriteria, cancellationToken);
            }
        }

        private async Task CreateNew(SaveCourseCriteriaCommand command, CancellationToken cancellationToken)
        {
            var courseCriteria = new CourseCriteria() { Id = command.Id };
            SetDataForCourseCriteriaEntity(courseCriteria, command);
            courseCriteria.CreatedDate = Clock.Now;
            courseCriteria.CreatedBy = CurrentUserIdOrDefault;

            await _courseCriteriaCudLogic.Insert(courseCriteria, cancellationToken);
        }

        private async Task Update(SaveCourseCriteriaCommand command, CourseCriteria courseCriteria, CancellationToken cancellationToken)
        {
            SetDataForCourseCriteriaEntity(courseCriteria, command);
            courseCriteria.ChangedDate = Clock.Now;
            courseCriteria.ChangedBy = CurrentUserId;

            await _courseCriteriaCudLogic.Update(courseCriteria, cancellationToken);
        }

        private void SetDataForCourseCriteriaEntity(CourseCriteria courseCriteria, SaveCourseCriteriaCommand command)
        {
            courseCriteria.AccountType = command.AccountType;
            courseCriteria.Tracks = command.Tracks;
            courseCriteria.DevRoles = command.DevRoles;
            courseCriteria.TeachingLevels = command.TeachingLevels;
            courseCriteria.TeachingCourseOfStudy = command.TeachingCourseOfStudy;
            courseCriteria.JobFamily = command.JobFamily;
            courseCriteria.CoCurricularActivity = command.CoCurricularActivity;
            courseCriteria.SubGradeBanding = command.SubGradeBanding;
            courseCriteria.CourseCriteriaServiceSchemes = command.CourseCriteriaServiceSchemes;
            courseCriteria.PlaceOfWork = command.PlaceOfWork;
            courseCriteria.PreRequisiteCourses = command.PreRequisiteCourses;
        }
    }
}
