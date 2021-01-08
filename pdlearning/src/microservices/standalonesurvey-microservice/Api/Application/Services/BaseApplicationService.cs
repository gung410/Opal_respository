using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Services
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
