using Thunder.Platform.Cqrs;

namespace Conexus.Opal.Microservice.Tagging.Services
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
