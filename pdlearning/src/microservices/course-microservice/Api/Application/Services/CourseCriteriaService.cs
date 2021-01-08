using System;
using System.Threading.Tasks;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries;
using Microservice.Course.Application.RequestDtos;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Services
{
    public class CourseCriteriaService : BaseApplicationService
    {
        public CourseCriteriaService(IThunderCqrs thunderCqrs, IUnitOfWorkManager unitOfWork) : base(thunderCqrs, unitOfWork)
        {
        }

        public Task<CourseCriteriaModel> GetCourseCriteriaByIdQuery(Guid courseCriteriaId)
        {
            return ThunderCqrs.SendQuery(new GetCourseCriteriaByIdQuery { Id = courseCriteriaId });
        }

        public async Task<CourseCriteriaModel> SaveCourseCriteria(SaveCourseCriteriaRequest request)
        {
            var command = request.Data.ToCommand();
            await ThunderCqrs.SendCommand(command);
            return await ThunderCqrs.SendQuery(new GetCourseCriteriaByIdQuery { Id = command.Id });
        }
    }
}
