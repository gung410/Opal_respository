using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Services
{
    public class BaseApplicationService
    {
        public BaseApplicationService(IThunderCqrs thunderCqrs)
        {
            ThunderCqrs = thunderCqrs;
        }

        protected IThunderCqrs ThunderCqrs { get; }
    }
}
