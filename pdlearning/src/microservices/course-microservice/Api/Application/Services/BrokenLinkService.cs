using System.Threading.Tasks;
using Microservice.Course.Application.Commands;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Services
{
    public class BrokenLinkService : BaseApplicationService
    {
        public BrokenLinkService(IThunderCqrs thunderCqrs, IUnitOfWorkManager unitOfWork) : base(thunderCqrs, unitOfWork)
        {
        }

        public Task ExtractAll()
        {
            var extractCommand = new ExtractContentUrlCommand();
            return ThunderCqrs.SendCommand(extractCommand);
        }
    }
}
